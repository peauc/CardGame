﻿namespace ServerTest
{
    using System;
    using System.Collections.Generic;

    using CardGame.Protocol;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Server.Game;

    using Announce = CardGame.Protocol.Announce;

    [TestClass]
    public class TrickManagerTest
    {
        private readonly CardManager cardManager;

        private readonly List<Player> players;

        private readonly Team[] teams;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly BiddingManager biddingManager;

        public TrickManagerTest()
        {
            this.cardManager = new CardManager();

            this.players = new List<Player>
                               {
                                   new Player(null, "player1"),
                                   new Player(null, "player2"),
                                   new Player(null, "player3"),
                                   new Player(null, "player4"),
                               };

            this.teams = new Team[2];
            this.teams[0] = new Team(this.players[0], this.players[2]);
            this.teams[1] = new Team(this.players[1], this.players[3]);
            this.teams[0].OppositeTeam = this.teams[1];
            this.teams[1].OppositeTeam = this.teams[0];
            this.players[0].Team = this.teams[0];
            this.players[1].Team = this.teams[1];
            this.players[2].Team = this.teams[0];
            this.players[3].Team = this.teams[1];

            this.cardManager.Mix(72);
            this.cardManager.DistributeToAll(this.players);

            this.biddingManager = new BiddingManager(this.teams, this.cardManager);
            this.biddingManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Contract, Contract = new Contract { Type = Contract.Types.Type.Spades, Score = 90 } });
            this.biddingManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Pass });
            this.biddingManager.HandleTurn(this.players[2], new Event { Type = Event.Types.Type.Pass });
            this.biddingManager.HandleTurn(this.players[3], new Event { Type = Event.Types.Type.Pass });

            this.cardManager.CurrentTrump = this.biddingManager.TrumpType;
        }

        [TestMethod]
        public void TrickManagerFirstValidTrickWithOnlyStartCard()
        {
            TrickManager trickManager = new TrickManager(this.teams, this.cardManager, 0);

            trickManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Hearts, Value = Card.Types.Value.Seven } });
            Assert.IsTrue(trickManager.Success);

            trickManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Hearts, Value = Card.Types.Value.Jack } });
            Assert.IsTrue(trickManager.Success);

            trickManager.HandleTurn(this.players[2], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Hearts, Value = Card.Types.Value.Ace } });
            Assert.IsTrue(trickManager.Success);

            trickManager.HandleTurn(this.players[3], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Hearts, Value = Card.Types.Value.King } });
            Assert.IsTrue(trickManager.Success);

            Assert.IsTrue(trickManager.HasEnded);
            Assert.AreEqual(1, this.teams[0].TricksWon);
            Assert.AreEqual(0, this.teams[1].TricksWon);
            Assert.AreEqual(17, this.teams[0].RoundScore);
            Assert.AreEqual(0, this.teams[1].RoundScore);
        }

        [TestMethod]
        public void TrickManagerLastValidTrickWithOnlyStartCard()
        {
            TrickManager trickManager = new TrickManager(this.teams, this.cardManager, 7);

            trickManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Hearts, Value = Card.Types.Value.Seven } });
            Assert.IsTrue(trickManager.Success);

            trickManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Hearts, Value = Card.Types.Value.Jack } });
            Assert.IsTrue(trickManager.Success);

            trickManager.HandleTurn(this.players[2], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Hearts, Value = Card.Types.Value.Ace } });
            Assert.IsTrue(trickManager.Success);

            trickManager.HandleTurn(this.players[3], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Hearts, Value = Card.Types.Value.King } });
            Assert.IsTrue(trickManager.Success);

            Assert.IsTrue(trickManager.HasEnded);
            Assert.AreEqual(1, this.teams[0].TricksWon);
            Assert.AreEqual(0, this.teams[1].TricksWon);
            Assert.AreEqual(27, this.teams[0].RoundScore);
            Assert.AreEqual(0, this.teams[1].RoundScore);
        }

        [TestMethod]
        public void TrickManagerFirstValidTrickWithTrumpCards()
        {
            this.cardManager.Mix(74);
            this.cardManager.DistributeToAll(this.players);

            TrickManager trickManager = new TrickManager(this.teams, this.cardManager, 0);

            trickManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.King } });
            Assert.IsTrue(trickManager.Success);

            trickManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.Seven } });
            Assert.IsTrue(trickManager.Success);

            trickManager.HandleTurn(this.players[2], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.Nine } });
            Assert.IsTrue(trickManager.Success);

            trickManager.HandleTurn(this.players[3], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Spades, Value = Card.Types.Value.Ten } });
            Assert.IsTrue(trickManager.Success);

            Assert.IsTrue(trickManager.HasEnded);
            Assert.AreEqual(0, this.teams[0].TricksWon);
            Assert.AreEqual(1, this.teams[1].TricksWon);
            Assert.AreEqual(0, this.teams[0].RoundScore);
            Console.WriteLine(this.teams[1].RoundScore);
            Assert.AreEqual(14, this.teams[1].RoundScore);
        }

        [TestMethod]
        public void TrickManagerValidBelote()
        {
            this.cardManager.Mix(78);
            this.cardManager.DistributeToAll(this.players);
            this.cardManager.CurrentTrump = Contract.Types.Type.Spades;

            this.players[2].Belote = Player.BeloteState.Done;

            TrickManager trickManager = new TrickManager(this.teams, this.cardManager, 0);

            trickManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.Nine } });
            Assert.IsTrue(trickManager.Success);

            trickManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Belote });

            trickManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Spades, Value = Card.Types.Value.Queen } });
            Assert.IsTrue(trickManager.Success);

            Assert.IsTrue(this.players[2].Belote == Player.BeloteState.Done);
        }

        [TestMethod]
        public void TrickManagerValidRebelote()
        {
            this.cardManager.Mix(77);
            this.cardManager.DistributeToAll(this.players);
            this.cardManager.CurrentTrump = Contract.Types.Type.Hearts;

            this.players[2].Belote = Player.BeloteState.Done;

            TrickManager trickManager = new TrickManager(this.teams, this.cardManager, 0);

            trickManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.Eight } });
            Assert.IsTrue(trickManager.Success);

            trickManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.Jack } });
            Assert.IsTrue(trickManager.Success);

            trickManager.HandleTurn(this.players[2], new Event { Type = Event.Types.Type.Rebelote });

            trickManager.HandleTurn(this.players[2], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Hearts, Value = Card.Types.Value.King } });
            Assert.IsTrue(trickManager.Success);

            Assert.IsTrue(this.players[2].Rebelote == Player.BeloteState.Done);
            Assert.AreEqual(20, this.teams[0].BonusRoundScore);
        }

        [TestMethod]
        public void TrickManagerValidAnnounce()
        {
            foreach (Player player in this.players)
            {
                Console.WriteLine($"\r\n{player.Name}");
                foreach (Card card in player.Hand.Card)
                {
                    Console.WriteLine($"{card.Value} of {card.Type}");
                }
            }

            TrickManager trickManager = new TrickManager(this.teams, this.cardManager, 0);

            trickManager.HandleTurn(
                this.players[0],
                new Event
                {
                    Type = Event.Types.Type.Announce,
                    Announce = new Announce
                    {
                        Type = Announce.Types.Type.Carre,
                        Card = new Card
                        {
                            Type = Card.Types.Type.Hearts,
                            Value = Card.Types.Value.Queen
                        }
                    }
                });
            Assert.AreEqual((uint)200, trickManager.Reply.Number);
        }

        [TestMethod]
        public void TrickManagerInvalidAnnounceNotFirstTrick()
        {
            this.cardManager.Mix(74);
            this.cardManager.DistributeToAll(this.players);

            TrickManager trickManager = new TrickManager(this.teams, this.cardManager, 1);

            trickManager.HandleTurn(
                this.players[0],
                new Event
                {
                    Type = Event.Types.Type.Announce,
                    Announce = new Announce
                    {
                        Type = Announce.Types.Type.Carre,
                        Card = new Card
                        {
                            Type = Card.Types.Type.Diamonds,
                            Value = Card.Types.Value.Jack
                        }
                    }
                });
            Assert.IsFalse(trickManager.Success);
        }

        [TestMethod]
        public void TrickManagerInvalidCardNotStartType()
        {
            this.cardManager.Mix(74);
            this.cardManager.DistributeToAll(this.players);

            TrickManager trickManager = new TrickManager(this.teams, this.cardManager, 0);

            trickManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.King } });
            Assert.IsTrue(trickManager.Success);

            trickManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Spades, Value = Card.Types.Value.Queen } });
            Assert.IsFalse(trickManager.Success);
        }

        [TestMethod]
        public void TrickManagerInvalidCardNotStartTypeAndNotTrumpType()
        {
            this.cardManager.Mix(74);
            this.cardManager.DistributeToAll(this.players);

            TrickManager trickManager = new TrickManager(this.teams, this.cardManager, 0);

            trickManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.King } });
            Assert.IsTrue(trickManager.Success);

            trickManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.Seven } });
            Assert.IsTrue(trickManager.Success);

            trickManager.HandleTurn(this.players[2], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.Nine } });
            Assert.IsTrue(trickManager.Success);

            trickManager.HandleTurn(this.players[3], new Event { Type = Event.Types.Type.Play, Card = new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.King } });
            Assert.IsFalse(trickManager.Success);
        }
    }
}