namespace ServerTest
{
    using System;
    using System.Linq;

    using CardGame.Protocol;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Server.Game;

    using Announce = Server.Game.Announce;

    [TestClass]
    public class RoundTest
    {
        [TestMethod]
        public void TeamGetBestAnnounce()
        {
            // ARRANGE
            Player player1 = new Player(null, "player1");
            Player player2 = new Player(null, "player2");
            Player player3 = new Player(null, "player3");
            Player player4 = new Player(null, "player4");

            Card card1 = new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.Ten };
            Card card2 = new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.King };

            Game game = new Game();

            game.PlayerManager.Players.Add(player1);
            game.PlayerManager.Players.Add(player2);
            game.PlayerManager.Players.Add(player3);
            game.PlayerManager.Players.Add(player4);

            game.Teams[0] = new Team(player1, player2);
            game.Teams[1] = new Team(player3, player4);
            game.Teams[0].OppositeTeam = game.Teams[1];
            game.Teams[1].OppositeTeam = game.Teams[0];

            Round round = new Round(game);

            // ACT
            Announce announce1 = new Announce(player1, AnnounceType.Carre, card1);
            Announce announce2 = new Announce(player2, AnnounceType.Carre, card2);
            Announce announce3 = new Announce(player1, AnnounceType.Tierce, card2);

            game.Teams[0].PrepareRound(false, false, false, null);
            game.Teams[1].PrepareRound(false, false, false, null);

            game.Teams[0].Announces.Add(announce1);
            game.Teams[1].Announces.Add(announce2);
            game.Teams[0].Announces.Add(announce3);

            round.SetupAnnounces();

            // ASSERT
            Assert.IsTrue(!game.Teams[0].Announces.Any());
            Assert.IsTrue(game.Teams[1].Announces.Any());
        }
    }
}