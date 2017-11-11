using System;
using System.Collections.Generic;
using DotNetty.Transport.Channels;

namespace Server
{
    public class GameManager
    {
        private List<Game.Game> GameList = new List<Game.Game>();

        private void CreateNewGame()
        {
            GameList.Add(new Game.Game());
        }

        public Game.Game GetFreeGame()
        {
            foreach (Game.Game g in GameList)
            {
                if (g.PlayerManager.Players.Count != 4)
                    return (g);
            }
            return (null);
        }

        public void AddPlayerToGame(Game.Player p)
        {
            Game.Game g;

            if ((g = GetFreeGame()) != null)
            {
                g.PlayerManager.Players.Add(p);
                return;
            }
            CreateNewGame();
            AddPlayerToGame(p);
        }

        public Game.Player FindPlayerByContext(IChannelHandlerContext ctx)
        {
            foreach (Game.Game g in GameList)
            {
                Game.Player p;
                if ((p = g.PlayerManager.ContainsPlayer(ctx)) != null)
                {
                    return (p);
                }
            }
            return (null);
        }

        public Game.Game FindGameByPlayer(Game.Player p)
        {
            foreach (Game.Game g in GameList)
            {
                if (g.PlayerManager.ContainsPlayer(p.Ctx) != null)
                    return (g);
            }
            return (null);
        }
    }
}