namespace Server.Game
{
    using CardGame.Protocol;

    /// <summary>
    /// The game.
    /// </summary>
    public partial class Game
    {
        public Game()
        {
            this.PlayerManager = new PlayerManager();
            this.ResetGame();
        }

        public PlayerManager PlayerManager { get; set; }

        public Team[] Teams { get; set; }

        public GameState State { get; private set; }

        public CardManager CardManager { get; private set; }

        private int CurrentPlayerIndex { get; set; }

        private Round CurrentRound { get; set; }

        private int CurrentRoundIndex { get; set; }

        public void HandlePlay(Message message, Player player)
        {
            if (message.Type == Message.Types.Type.Event && message.Event.Type == Event.Types.Type.Name)
            {
                this.PlayerManager.ChangeNameAndTellPlayers(player, message.Event);
                if (this.PlayerManager.PlayersAreSet())
                {
                    this.Start();
                }

                return;
            }

            if (this.State != GameState.Game)
            {
                player.Reply(402, "The game is not ready.");
            }
            else if (HandleCommonCommands(message, player))
            {
            }
            else
            {
                if (player.Name != this.PlayerManager.Players[this.CurrentPlayerIndex].Name)
                {
                    player.Reply(403, "It's not your turn to play.");
                }
                else
                {
                    if (this.CurrentRound.HandlePlayerMessage(player, message.Event))
                    {
                        this.CurrentPlayerIndex++;
                        if (this.CurrentPlayerIndex >= this.PlayerManager.Players.Count)
                        {
                            this.CurrentPlayerIndex = 0;
                        }

                        this.PlayerManager.Players[this.CurrentPlayerIndex]
                            .Prompt("It's your turn to play, here is your hand:");
                        this.PlayerManager.Players[this.CurrentPlayerIndex].SendHand();
                        if (this.CurrentRound.HasEnded)
                        {
                            if (this.Teams[0].TotalScore >= 3000 || this.Teams[1].TotalScore >= 3000)
                            {
                                this.State = GameState.Scores;
                                ScoreManager.ShowScores(this.Teams, this.PlayerManager);
                            }
                            else
                            {
                                this.CurrentRoundIndex++;
                                this.CurrentRound = new Round(this.Teams, this.CardManager, this.CurrentRoundIndex, this);
                            }
                        }
                    }
                }
            }
        }

        private static bool HandleCommonCommands(Message message, Player player)
        {
            if (message.Type != Message.Types.Type.Event || message.Event == null)
            {
                player.Reply(402, "Invalid message.");
                return true;
            }

            if (message.Event.Type == Event.Types.Type.Hand)
            {
                player.Reply(200, "SUCCESS");
                player.SendHand();
                return true;
            }

            return false;
        }

        public void ResetGame()
        {
            this.Teams = new Team[2];
            this.State = GameState.AwaitingPlayers;
            this.CardManager = new CardManager();
            this.PlayerManager.SetupForNewRound();
        }

        private void Start()
        {
            this.CreateTeams();
            this.PlayerManager.SetupForNewRound();
            this.CurrentRound = new Round(this.Teams, this.CardManager, 0, this);
            this.CurrentRoundIndex = 0;
            this.CurrentPlayerIndex = 0;
            this.PlayerManager.Players[this.CurrentPlayerIndex]
                .Prompt("It's your turn to play, here is your hand:");
            this.PlayerManager.Players[this.CurrentPlayerIndex].SendHand();
            this.State = GameState.Game;
        }

        private void CreateTeams()
        {
            this.Teams[0] = new Team(this.PlayerManager.Players[0], this.PlayerManager.Players[2]);
            this.Teams[1] = new Team(this.PlayerManager.Players[1], this.PlayerManager.Players[3]);
            this.Teams[0].OppositeTeam = this.Teams[1];
            this.Teams[1].OppositeTeam = this.Teams[0];
            this.PlayerManager.Players[0].Team = this.Teams[0];
            this.PlayerManager.Players[1].Team = this.Teams[1];
            this.PlayerManager.Players[2].Team = this.Teams[0];
            this.PlayerManager.Players[3].Team = this.Teams[1];
            this.PlayerManager.PromptToAll($"Team 1 : {this.PlayerManager.Players[0].Name} and {this.PlayerManager.Players[2].Name}");
            this.PlayerManager.PromptToAll($"Team 2 : {this.PlayerManager.Players[1].Name} and {this.PlayerManager.Players[3].Name}");
        }
    }
}