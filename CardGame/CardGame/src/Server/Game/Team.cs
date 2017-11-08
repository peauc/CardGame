namespace CardGame.Server.Game
{
    using System.Linq;

    using CardGame.Protocol;

    public class Team
    {
        public Team(Player player1, Player player2)
        {
            this.Players = new Player[2];
            this.Players[0] = player1;
            this.Players[1] = player2;
            this.Players[0].Team = this;
            this.Players[1].Team = this;
        }

        public Player[] Players { get; }

        public bool IsCapot { get; set; }

        public bool HasCoinched { get; set; }

        public bool HasSurcoinched { get; set; }

        public long TotalScore { get; set; }

        public long RoundScore { get; set; }

        public long BonusRoundScore { get; set; }

        public int TricksWon { get; set; }

        public Contract Contract { get; set; }

        //// ANNOUNCES
        public void PrepareRound(bool isCapot, bool hasCoinched, bool hasSurcoinched, Contract contract)
        {
            this.TricksWon = 0;
            this.IsCapot = isCapot;
            this.HasCoinched = hasCoinched;
            this.HasSurcoinched = hasSurcoinched;
            this.Contract = contract;

            //// ANNOUNCES
        }

        public bool IsMember(Player player)
        {
            return this.Players.Any(teamPlayer => teamPlayer.Name == player.Name);
        }

        //// ANNOUNCES
        public bool HasValidatedContract()
        {
            if (this.Contract != null)
            {
                if (this.IsCapot)
                {
                    return this.TricksWon == 8;
                }

                return this.RoundScore >= this.Contract.Score;
            }

            return false;
        }

        public void AwardContractPoints()
        {
            if (this.IsCapot)
            {
                this.TotalScore += this.RoundScore + 500;
            }
            else
            {
                this.TotalScore += this.RoundScore + this.Contract.Score;
            }
        }
    }
}