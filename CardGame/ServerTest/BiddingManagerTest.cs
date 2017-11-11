namespace ServerTest
{
    using System.Collections.Generic;

    using CardGame.Protocol;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Server.Game;

    [TestClass]
    public class BiddingManagerTest
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly CardManager cardManager;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private List<Player> players;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private Team[] teams;

        public BiddingManagerTest()
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
        }

        [TestMethod]
        public void BiddingManagerInvalidCommand()
        {
            BiddingManager biddingManager = new BiddingManager(this.teams);

            biddingManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Name, Argument = { "Denis" } });
            Assert.IsFalse(biddingManager.Success);
        }

        [TestMethod]
        public void BiddingManagerValidSimpleContract()
        {
            BiddingManager biddingManager = new BiddingManager(this.teams);

            biddingManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Contract, Contract = new Contract { Type = Contract.Types.Type.Diamonds, Score = 90 } });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Pass });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[2], new Event { Type = Event.Types.Type.Pass });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[3], new Event { Type = Event.Types.Type.Pass });
            Assert.IsTrue(biddingManager.Success);

            Assert.IsTrue(biddingManager.HasEnded);
            Assert.IsTrue(biddingManager.TrumpType == Contract.Types.Type.Diamonds);
            Assert.IsTrue(this.teams[0].Contract != null);
            Assert.IsFalse(this.teams[1].Contract != null);
        }

        [TestMethod]
        public void BiddingManagerValidCapot()
        {
            BiddingManager biddingManager = new BiddingManager(this.teams);

            biddingManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Contract, Contract = new Contract { Type = Contract.Types.Type.Diamonds, Score = 250 } });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Pass });
            Assert.IsTrue(biddingManager.Success);

            Assert.IsTrue(biddingManager.HasEnded);
            Assert.IsTrue(biddingManager.TrumpType == Contract.Types.Type.Diamonds);
            Assert.IsTrue(this.teams[0].IsCapot);
            Assert.IsTrue(this.teams[0].Contract != null);
            Assert.IsFalse(this.teams[1].Contract != null);
        }

        [TestMethod]
        public void BiddingManagerValidCoinche()
        {
            BiddingManager biddingManager = new BiddingManager(this.teams);

            biddingManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Contract, Contract = new Contract { Type = Contract.Types.Type.Diamonds, Score = 90 } });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Coinche });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[2], new Event { Type = Event.Types.Type.Pass });
            Assert.IsTrue(biddingManager.Success);

            Assert.IsTrue(biddingManager.HasEnded);
            Assert.IsTrue(biddingManager.TrumpType == Contract.Types.Type.Diamonds);
            Assert.IsTrue(this.teams[0].Contract != null);
            Assert.IsFalse(this.teams[1].Contract != null);
            Assert.IsTrue(!this.teams[0].HasSurcoinched);
            Assert.IsTrue(this.teams[1].HasCoinched);
        }

        [TestMethod]
        public void BiddingManagerValidSurcoinche()
        {
            BiddingManager biddingManager = new BiddingManager(this.teams);

            biddingManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Contract, Contract = new Contract { Type = Contract.Types.Type.Diamonds, Score = 90 } });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Coinche });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[2], new Event { Type = Event.Types.Type.Surcoinche });
            Assert.IsTrue(biddingManager.Success);

            Assert.IsTrue(biddingManager.HasEnded);
            Assert.IsTrue(biddingManager.TrumpType == Contract.Types.Type.Diamonds);
            Assert.IsTrue(this.teams[0].Contract != null);
            Assert.IsFalse(this.teams[1].Contract != null);
            Assert.IsTrue(this.teams[0].HasSurcoinched);
            Assert.IsTrue(!this.teams[1].HasCoinched);
        }

        [TestMethod]
        public void BiddingManagerInvalidCoincheBecauseOfNoContractToCoinche()
        {
            BiddingManager biddingManager = new BiddingManager(this.teams);

            biddingManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Coinche });
            Assert.IsFalse(biddingManager.Success);
        }

        [TestMethod]
        public void BiddingManagerInvalidCoincheBecauseOfState()
        {
            BiddingManager biddingManager = new BiddingManager(this.teams);

            biddingManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Contract, Contract = new Contract { Type = Contract.Types.Type.Diamonds, Score = 90 } });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Coinche });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[2], new Event { Type = Event.Types.Type.Coinche });
            Assert.IsFalse(biddingManager.Success);
        }

        [TestMethod]
        public void BiddingManagerInvalidSurcoincheBecauseOfState()
        {
            BiddingManager biddingManager = new BiddingManager(this.teams);

            biddingManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Contract, Contract = new Contract { Type = Contract.Types.Type.Diamonds, Score = 90 } });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Surcoinche });
            Assert.IsFalse(biddingManager.Success);
        }

        [TestMethod]
        public void BiddingManagerInvalidContractsBecauseOfScore()
        {
            BiddingManager biddingManager = new BiddingManager(this.teams);

            biddingManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Pass });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Contract, Contract = new Contract { Type = Contract.Types.Type.Diamonds, Score = 60 } });
            Assert.IsFalse(biddingManager.Success);

            biddingManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Contract, Contract = new Contract { Type = Contract.Types.Type.Diamonds, Score = 260 } });
            Assert.IsFalse(biddingManager.Success);

            biddingManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Contract, Contract = new Contract { Type = Contract.Types.Type.Diamonds, Score = 100 } });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[2], new Event { Type = Event.Types.Type.Contract, Contract = new Contract { Type = Contract.Types.Type.Clubs, Score = 140 } });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[3], new Event { Type = Event.Types.Type.Contract, Contract = new Contract { Type = Contract.Types.Type.Diamonds, Score = 120 } });
            Assert.IsFalse(biddingManager.Success);

            biddingManager.HandleTurn(this.players[3], new Event { Type = Event.Types.Type.Pass });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Pass });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Pass });
            Assert.IsTrue(biddingManager.Success);

            Assert.IsTrue(biddingManager.HasEnded);
            Assert.IsTrue(biddingManager.TrumpType == Contract.Types.Type.Clubs);
            Assert.IsTrue(this.teams[0].Contract != null);
            Assert.IsFalse(this.teams[1].Contract != null);
        }

        [TestMethod]
        public void BiddingManagerInvalidContractsBecauseOfCapot()
        {
            BiddingManager biddingManager = new BiddingManager(this.teams);

            biddingManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Contract, Contract = new Contract { Type = Contract.Types.Type.Diamonds, Score = 250 } });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Contract, Contract = new Contract { Type = Contract.Types.Type.Diamonds, Score = 100 } });
            Assert.IsFalse(biddingManager.Success);
        }

        [TestMethod]
        public void BiddingManagerInvalidContractsBecauseOfCoinche()
        {
            BiddingManager biddingManager = new BiddingManager(this.teams);

            biddingManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Contract, Contract = new Contract { Type = Contract.Types.Type.Diamonds, Score = 100 } });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Coinche });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[2], new Event { Type = Event.Types.Type.Contract, Contract = new Contract { Type = Contract.Types.Type.Diamonds, Score = 130 } });
            Assert.IsFalse(biddingManager.Success);
        }

        [TestMethod]
        public void BiddingManagerNotEndedBecauseOfNoContract()
        {
            BiddingManager biddingManager = new BiddingManager(this.teams);

            biddingManager.HandleTurn(this.players[0], new Event { Type = Event.Types.Type.Pass });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[1], new Event { Type = Event.Types.Type.Pass });
            Assert.IsTrue(biddingManager.Success);

            biddingManager.HandleTurn(this.players[2], new Event { Type = Event.Types.Type.Pass });
            Assert.IsTrue(biddingManager.Success);

            Assert.IsFalse(biddingManager.HasEnded);

            biddingManager.HandleTurn(this.players[3], new Event { Type = Event.Types.Type.Pass });
            Assert.IsTrue(biddingManager.Success);

            Assert.IsFalse(biddingManager.HasEnded);
        }
    }
}