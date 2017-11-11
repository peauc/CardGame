namespace Server.Game
{
    using System.Collections.Generic;
    using System.Linq;

    using CardGame.Protocol;

    using DotNetty.Transport.Channels;

    public class PlayerManager
    {
        public PlayerManager()
        {
            this.Players = new List<Player>();
        }

        public List<Player> Players { get; set; }

        public void PromptToAll(string str)
        {
            foreach (Player player in this.Players)
            {
                player.Prompt(str);
            }
        }

        public void PromptToAllButPlayer(string str, Player p)
        {
            foreach (Player player in this.Players)
            {
                if (p.Ctx != player.Ctx)
                {
                    player.Prompt(str);
                }
            }
        }

        public void SetupForNewRound()
        {
            foreach (Player player in this.Players)
            {
                player.SetupForNewRound();
            }
        }

        public bool IsNameTaken(string name)
        {
            return this.Players.Any(player => player.Name == name);
        }

        public bool PlayersAreSet()
        {
            return this.Players.Count == 4 && this.Players.All(player => player.Name == string.Empty);
        }

        public Player ContainsPlayer(IChannelHandlerContext ctx)
        {
            return this.Players.FirstOrDefault(player => player.Ctx == ctx);
        }

        public Player ContainsPlayer(string name)
        {
            return this.Players.FirstOrDefault(player => player.Name == name);
        }

        public void ChangeNameAndTellPlayers(Player player, Event message)
        {
            if (message.Argument[0] == string.Empty)
            {
                player.Reply(400, "You thought that would not work didn't you.");
                return;
            }

            if (this.IsNameTaken(message.Argument[0]))
            {
                player.Reply(400, "Someone uses that username already.");
                return;
            }

            string exampleName = player.Name == string.Empty ? $"Player{this.Players.IndexOf(player)}" : player.Name;
            this.PromptToAllButPlayer($"{exampleName} is now known as {message.Argument[0]}", player);
            player.Name = message.Argument[0];
            player.Reply(200, $"You are now known as {player.Name}");
        }
    }
}