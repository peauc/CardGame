namespace ServerTest
{
    using System;

    using CardGame.Protocol;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Server.Game;

    using Announce = Server.Game.Announce;

    [TestClass]
    public class TeamTest
    {
        [TestMethod]
        public void TeamGetBestAnnounce()
        {
            // ARRANGE
            Player player1 = new Player(null, "player1");
            Player player2 = new Player(null, "player2");
            Team team = new Team(player1, player2);
            Card card1 = new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.Ten };
            Card card2 = new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.King };

            // ACT
            Announce announce1 = new Announce(player1, AnnounceType.Carre, card1);
            Announce announce2 = new Announce(player2, AnnounceType.Carre, card2);
            Announce announce3 = new Announce(player1, AnnounceType.Tierce, card2);

            team.PrepareRound(false, false, false, null);

            team.Announces.Add(announce1);
            team.Announces.Add(announce2);
            team.Announces.Add(announce3);

            // ASSERT
            Console.WriteLine(team.GetBestAnnounce().Type.ToString());
            Assert.AreEqual(announce2, team.GetBestAnnounce());
        }
    }
}