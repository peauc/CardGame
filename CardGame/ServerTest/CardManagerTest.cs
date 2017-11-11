namespace ServerTest
{
    using System.Collections.Generic;

    using CardGame.Protocol;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Server.Game;

    [TestClass]
    public class CardManagerTest
    {
        [TestMethod]
        public void CardManagerDistributeToAll()
        {
            // ARRANGE
            CardManager cardManager = new CardManager();
            List<Player> players = new List<Player>
                                       {
                                           new Player(null, "player1"),
                                           new Player(null, "player2"),
                                           new Player(null, "player3"),
                                           new Player(null, "player4"),
                                       };

            // ACT
            cardManager.Mix();
            cardManager.DistributeToAll(players);

            // ASSERT
            Assert.IsTrue(players[0].Hand.Card.Count == 8);
            Assert.IsTrue(players[1].Hand.Card.Count == 8);
            Assert.IsTrue(players[2].Hand.Card.Count == 8);
            Assert.IsTrue(players[3].Hand.Card.Count == 8);
        }

        [TestMethod]
        public void CardManagerGetCurrentTurnWinner()
        {
            // ARRANGE
            CardManager cardManager = new CardManager();
            List<Player> players = new List<Player>
                                       {
                                           new Player(null, "player1"),
                                           new Player(null, "player2"),
                                           new Player(null, "player3"),
                                           new Player(null, "player4"),
                                       };
            IDictionary<Player, Card> cards = new Dictionary<Player, Card>
                                                  {
                                                      {
                                                          players[0],
                                                          new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.King }
                                                      },
                                                      {
                                                          players[1],
                                                          new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.Jack }
                                                      },
                                                      {
                                                          players[2],
                                                          new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.Ace }
                                                      },
                                                      {
                                                          players[3],
                                                          new Card { Type = Card.Types.Type.Hearts, Value = Card.Types.Value.Nine }
                                                      }
                                                  };

            // ACT
            cardManager.CurrentTrump = Contract.Types.Type.Spades;
            Assert.AreEqual(cardManager.GetCurrentTurnWinner(cards), players[2]);
            cardManager.CurrentTrump = Contract.Types.Type.Clubs;
            Assert.AreEqual(cardManager.GetCurrentTurnWinner(cards), players[1]);
            cardManager.CurrentTrump = Contract.Types.Type.Diamonds;
            Assert.AreEqual(cardManager.GetCurrentTurnWinner(cards), players[0]);
        }
    }
}