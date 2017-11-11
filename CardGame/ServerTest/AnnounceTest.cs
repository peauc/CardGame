namespace ServerTest
{
    using System;
    using System.Collections.Generic;

    using CardGame.Protocol;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Server.Game;

    using Announce = Server.Game.Announce;

    [TestClass]
    public class AnnounceTest
    {
        [TestMethod]
        public void AnnounceConstructorCarreKingDiamonds()
        {
            // ARRANGE
            Player player = new Player(null, "player1");
            Card card = new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.King };
            List<Announce.AnnounceCard> announceCards = new List<Announce.AnnounceCard>
                                                            {
                                                                new Announce.AnnounceCard(new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.King }),
                                                                new Announce.AnnounceCard(new Card { Type = Card.Types.Type.Hearts, Value = Card.Types.Value.King }),
                                                                new Announce.AnnounceCard(new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.King }),
                                                                new Announce.AnnounceCard(new Card { Type = Card.Types.Type.Spades, Value = Card.Types.Value.King })
                                                            };

            // ACT
            Announce announce = new Announce(player, AnnounceType.Carre, card);

            // ASSERT
            for (int i = 0; i < announceCards.Count; i++)
            {
                Assert.AreEqual(announceCards[i].Card.Value, announce.CardsToValidate[i].Card.Value);
                Assert.AreEqual(announceCards[i].Card.Type, announce.CardsToValidate[i].Card.Type);
            }
        }

        [TestMethod]
        public void AnnounceConstructorCentKingDiamonds()
        {
            // ARRANGE
            Player player = new Player(null, "player1");
            Card card = new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.King };
            List<Announce.AnnounceCard> announceCards = new List<Announce.AnnounceCard>
                                                            {
                                                                new Announce.AnnounceCard(new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.Nine }),
                                                                new Announce.AnnounceCard(new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.Ten }),
                                                                new Announce.AnnounceCard(new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.Jack }),
                                                                new Announce.AnnounceCard(new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.Queen }),
                                                                new Announce.AnnounceCard(new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.King })
                                                            };

            // ACT
            Announce announce = new Announce(player, AnnounceType.Cent, card);

            // ASSERT
            for (int i = 0; i < announceCards.Count; i++)
            {
                Assert.AreEqual(announceCards[i].Card.Value, announce.CardsToValidate[i].Card.Value);
                Assert.AreEqual(announceCards[i].Card.Type, announce.CardsToValidate[i].Card.Type);
            }
        }

        [TestMethod]
        public void AnnounceConstructorCentTenDiamonds()
        {
            // ARRANGE
            Player player = new Player(null, "player1");
            Card card = new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.Ten };

            // ACT
            try
            {
                Announce unused = new Announce(player, AnnounceType.Cent, card);

                // ASSERT
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                Console.WriteLine("yay");
            }
        }

        [TestMethod]
        public void AnnounceComparisons()
        {
            // ARRANGE
            Player player = new Player(null, "player1");
            Card card1 = new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.Ten };
            Card card2 = new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.King };

            // ACT
            Announce announce1 = new Announce(player, AnnounceType.Carre, card1);
            Announce announce2 = new Announce(player, AnnounceType.Carre, card2);
            Announce announce3 = new Announce(player, AnnounceType.Tierce, card2);

            // ASSERT
            Console.WriteLine(announce1.CompareTo(announce2));
            if (announce1.CompareTo(announce2) > 0)
            {
                Assert.Fail();
            }
            else if (announce2.CompareTo(announce3) < 0)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void AnnounceValidationOrderDoesNotMatter()
        {
            // ARRANGE
            Player player = new Player(null, "player1");
            Card card1 = new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.King };
            Card card2 = new Card { Type = Card.Types.Type.Hearts, Value = Card.Types.Value.King };
            Card card3 = new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.King };
            Card card4 = new Card { Type = Card.Types.Type.Spades, Value = Card.Types.Value.King };
            Card card5 = new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.Queen };

            // ACT
            Announce announce = new Announce(player, AnnounceType.Carre, card1);

            // ASSERT
            Assert.IsTrue(announce.Validate(card1));
            Assert.IsTrue(announce.Validate(card2));
            Assert.IsTrue(announce.Validate(card3));

            Assert.IsFalse(announce.IsComplete());

            Assert.IsFalse(announce.Validate(card5));

            Assert.IsTrue(announce.Validate(card4));

            Assert.IsTrue(announce.IsComplete());
        }

        [TestMethod]
        public void AnnounceValidationOrderDoesMatter()
        {
            // ARRANGE
            Player player = new Player(null, "player1");
            Card card1 = new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.King };
            Card card2 = new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.Queen };
            Card card3 = new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.Jack };
            Card card4 = new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.Nine };

            // ACT
            Announce announce = new Announce(player, AnnounceType.Tierce, card1);
            foreach (Announce.AnnounceCard announceCard in announce.CardsToValidate)
            {
                Console.WriteLine(announceCard.Card.Value.ToString());
            }

            // ASSERT
            Assert.IsTrue(announce.Validate(card3));
            Assert.IsFalse(announce.Validate(card1));
            Assert.IsTrue(announce.Validate(card2));
            Assert.IsFalse(announce.Validate(card4));
            Assert.IsTrue(announce.Validate(card1));

            Assert.IsTrue(announce.IsComplete());
        }
    }
}