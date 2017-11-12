namespace ServerTest
{
    using CardGame.Protocol;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Server.Game;

    using Announce = Server.Game.Announce;

    [TestClass]
    public class ScoreManagerTest
    {
        private readonly Game game;

        public ScoreManagerTest()
        {
            Player player1 = new Player(null, "player1");
            Player player2 = new Player(null, "player2");
            Player player3 = new Player(null, "player3");
            Player player4 = new Player(null, "player4");

            this.game = new Game();

            this.game.PlayerManager.Players.Add(player1);
            this.game.PlayerManager.Players.Add(player2);
            this.game.PlayerManager.Players.Add(player3);
            this.game.PlayerManager.Players.Add(player4);

            this.game.Teams[0] = new Team(player1, player2);
            this.game.Teams[1] = new Team(player3, player4);
            this.game.Teams[0].OppositeTeam = this.game.Teams[1];
            this.game.Teams[1].OppositeTeam = this.game.Teams[0];
        }

        [TestMethod]
        public void ScoreManagerCountScoreContractValidatedNoAnnounce()
        {
            // ARRANGE
            this.game.Teams[0].PrepareRound(false, false, false, null);
            this.game.Teams[1].PrepareRound(false, false, false, null);

            this.game.CardManager.CurrentTrump = Contract.Types.Type.Diamonds;
            this.game.Teams[0].Contract = new Contract { Score = 80, Type = Contract.Types.Type.Diamonds };
            this.game.Teams[1].Contract = null;

            this.game.Teams[0].RoundScore = 90;
            this.game.Teams[1].RoundScore = 40;

            // ACT
            ScoreManager.CountScores(this.game.PlayerManager, this.game.Teams);

            // ASSERT
            Assert.AreEqual(170, this.game.Teams[0].TotalScore);
            Assert.AreEqual(40, this.game.Teams[1].TotalScore);
        }

        [TestMethod]
        public void ScoreManagerCountScoreContractNotValidatedNoAnnounce()
        {
            // ARRANGE
            this.game.Teams[0].PrepareRound(false, false, false, null);
            this.game.Teams[1].PrepareRound(false, false, false, null);

            this.game.CardManager.CurrentTrump = Contract.Types.Type.Diamonds;
            this.game.Teams[0].Contract = new Contract { Score = 100, Type = Contract.Types.Type.Diamonds };
            this.game.Teams[1].Contract = null;

            this.game.Teams[0].RoundScore = 90;
            this.game.Teams[1].RoundScore = 40;

            // ACT
            ScoreManager.CountScores(this.game.PlayerManager, this.game.Teams);

            // ASSERT
            Assert.AreEqual(0, this.game.Teams[0].TotalScore);
            Assert.AreEqual(140, this.game.Teams[1].TotalScore);
        }

        [TestMethod]
        public void ScoreManagerCountScoreContractValidatedAnnounceValidated()
        {
            // ARRANGE
            Card card1 = new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.Ten };

            Announce announce1 = new Announce(this.game.PlayerManager.Players[0], AnnounceType.Carre, card1);

            this.game.Teams[0].PrepareRound(false, false, false, null);
            this.game.Teams[1].PrepareRound(false, false, false, null);

            this.game.CardManager.CurrentTrump = Contract.Types.Type.Diamonds;
            this.game.Teams[0].Contract = new Contract { Score = 80, Type = Contract.Types.Type.Diamonds };
            this.game.Teams[1].Contract = null;

            this.game.Teams[0].RoundScore = 90;
            this.game.Teams[1].RoundScore = 40;

            this.game.Teams[0].Announces.Add(announce1);
            this.game.Teams[0].Announces[0]
                .Validate(new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.Ten });
            this.game.Teams[0].Announces[0]
                .Validate(new Card { Type = Card.Types.Type.Hearts, Value = Card.Types.Value.Ten });
            this.game.Teams[0].Announces[0]
                .Validate(new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.Ten });
            this.game.Teams[0].Announces[0]
                .Validate(new Card { Type = Card.Types.Type.Spades, Value = Card.Types.Value.Ten });

            // ACT
            ScoreManager.CountScores(this.game.PlayerManager, this.game.Teams);

            // ASSERT
            Assert.AreEqual(270, this.game.Teams[0].TotalScore);
            Assert.AreEqual(40, this.game.Teams[1].TotalScore);
        }

        [TestMethod]
        public void ScoreManagerCountScoreContractNotValidatedAnnounceNotValidated()
        {
            // ARRANGE
            Card card1 = new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.Ten };

            Announce announce1 = new Announce(this.game.PlayerManager.Players[0], AnnounceType.Carre, card1);

            this.game.Teams[0].PrepareRound(false, false, false, null);
            this.game.Teams[1].PrepareRound(false, false, false, null);

            this.game.CardManager.CurrentTrump = Contract.Types.Type.Diamonds;
            this.game.Teams[0].Contract = new Contract { Score = 80, Type = Contract.Types.Type.Diamonds };
            this.game.Teams[1].Contract = null;

            this.game.Teams[0].RoundScore = 90;
            this.game.Teams[1].RoundScore = 40;

            this.game.Teams[0].Announces.Add(announce1);
            this.game.Teams[0].Announces[0]
                .Validate(new Card { Type = Card.Types.Type.Diamonds, Value = Card.Types.Value.Ten });
            this.game.Teams[0].Announces[0]
                .Validate(new Card { Type = Card.Types.Type.Hearts, Value = Card.Types.Value.Ten });
            this.game.Teams[0].Announces[0]
                .Validate(new Card { Type = Card.Types.Type.Clubs, Value = Card.Types.Value.Ten });

            // ACT
            ScoreManager.CountScores(this.game.PlayerManager, this.game.Teams);

            // ASSERT
            Assert.AreEqual(170, this.game.Teams[0].TotalScore);
            Assert.AreEqual(140, this.game.Teams[1].TotalScore);
        }

        [TestMethod]
        public void ScoreManagerCountScoreContractValidatedRebelote()
        {
            // ARRANGE
            this.game.Teams[0].PrepareRound(false, false, false, null);
            this.game.Teams[1].PrepareRound(false, false, false, null);

            this.game.CardManager.CurrentTrump = Contract.Types.Type.Diamonds;
            this.game.Teams[0].Contract = new Contract { Score = 80, Type = Contract.Types.Type.Diamonds };
            this.game.Teams[1].Contract = null;

            this.game.Teams[0].RoundScore = 90;
            this.game.Teams[1].RoundScore = 40;

            this.game.Teams[0].Players[0].Rebelote = Player.BeloteState.Done;
            this.game.Teams[0].BonusRoundScore = 20;

            // ACT
            ScoreManager.CountScores(this.game.PlayerManager, this.game.Teams);

            // ASSERT
            Assert.AreEqual(190, this.game.Teams[0].TotalScore);
            Assert.AreEqual(40, this.game.Teams[1].TotalScore);
        }

        [TestMethod]
        public void ScoreManagerCountScoreCapotValidated()
        {
            // ARRANGE
            this.game.Teams[0].PrepareRound(false, false, false, null);
            this.game.Teams[1].PrepareRound(false, false, false, null);

            this.game.CardManager.CurrentTrump = Contract.Types.Type.Diamonds;
            this.game.Teams[0].Contract = new Contract { Score = 250, Type = Contract.Types.Type.Diamonds };
            this.game.Teams[0].IsCapot = true;
            this.game.Teams[1].Contract = null;

            this.game.Teams[0].TricksWon = 8;
            this.game.Teams[0].RoundScore = 140;
            this.game.Teams[1].RoundScore = 0;

            // ACT
            ScoreManager.CountScores(this.game.PlayerManager, this.game.Teams);

            // ASSERT
            Assert.AreEqual(640, this.game.Teams[0].TotalScore);
            Assert.AreEqual(0, this.game.Teams[1].TotalScore);
        }

        [TestMethod]
        public void ScoreManagerCountScoreCapotNotValidated()
        {
            // ARRANGE
            this.game.Teams[0].PrepareRound(false, false, false, null);
            this.game.Teams[1].PrepareRound(false, false, false, null);

            this.game.CardManager.CurrentTrump = Contract.Types.Type.Diamonds;
            this.game.Teams[0].Contract = new Contract { Score = 250, Type = Contract.Types.Type.Diamonds };
            this.game.Teams[0].IsCapot = true;
            this.game.Teams[1].Contract = null;

            this.game.Teams[0].TricksWon = 7;
            this.game.Teams[0].RoundScore = 130;
            this.game.Teams[1].RoundScore = 10;

            // ACT
            ScoreManager.CountScores(this.game.PlayerManager, this.game.Teams);

            // ASSERT
            Assert.AreEqual(0, this.game.Teams[0].TotalScore);
            Assert.AreEqual(510, this.game.Teams[1].TotalScore);
        }

        [TestMethod]
        public void ScoreManagerCountScoreContractValidatedCoinche()
        {
            // ARRANGE
            this.game.Teams[0].PrepareRound(false, false, false, null);
            this.game.Teams[1].PrepareRound(false, false, false, null);

            this.game.CardManager.CurrentTrump = Contract.Types.Type.Diamonds;
            this.game.Teams[0].Contract = new Contract { Score = 80, Type = Contract.Types.Type.Diamonds };
            this.game.Teams[1].HasCoinched = true;
            this.game.Teams[1].Contract = null;

            this.game.Teams[0].RoundScore = 90;
            this.game.Teams[1].RoundScore = 30;

            // ACT
            ScoreManager.CountScores(this.game.PlayerManager, this.game.Teams);

            // ASSERT
            Assert.AreEqual(340, this.game.Teams[0].TotalScore);
            Assert.AreEqual(30, this.game.Teams[1].TotalScore);
        }

        [TestMethod]
        public void ScoreManagerCountScoreContractValidatedSurcoinche()
        {
            // ARRANGE
            this.game.Teams[0].PrepareRound(false, false, false, null);
            this.game.Teams[1].PrepareRound(false, false, false, null);

            this.game.CardManager.CurrentTrump = Contract.Types.Type.Diamonds;
            this.game.Teams[0].Contract = new Contract { Score = 80, Type = Contract.Types.Type.Diamonds };
            this.game.Teams[0].HasSurcoinched = true;
            this.game.Teams[1].Contract = null;

            this.game.Teams[0].RoundScore = 90;
            this.game.Teams[1].RoundScore = 30;

            // ACT
            ScoreManager.CountScores(this.game.PlayerManager, this.game.Teams);

            // ASSERT
            Assert.AreEqual(680, this.game.Teams[0].TotalScore);
            Assert.AreEqual(30, this.game.Teams[1].TotalScore);
        }
    }
}