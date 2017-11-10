namespace Server.Game
{
    using System.Collections.Generic;
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
            this.TotalScore = 0;
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

        public Team OppositeTeam { get; set; }

        public List<Announce> Announces { get; set; }

        public void PrepareRound(bool isCapot, bool hasCoinched, bool hasSurcoinched, Contract contract)
        {
            this.TricksWon = 0;
            this.IsCapot = isCapot;
            this.HasCoinched = hasCoinched;
            this.HasSurcoinched = hasSurcoinched;
            this.Contract = contract;
            this.RoundScore = 0;
            this.BonusRoundScore = 0;
            this.Announces = new List<Announce>();
        }

        public bool IsMember(Player player)
        {
            return this.Players.Any(teamPlayer => teamPlayer.Name == player.Name);
        }

        public int ValidateAnnounces(int bonus, List<Player> players)
        {
            int ownScore = 0;
            int otherScore = 0;

            this.RoundScore += bonus;

            if (this.Announces == null || !this.Announces.Any())
            {
                return 0;
            }

            foreach (Announce announce in this.Announces)
            {
                if (announce.IsComplete())
                {
                    foreach (Player player in players)
                    {
                        player.Prompt(
                            $"{((player.Team == this) ? "Your team" : "The opposite team")} has completed an announce. (type: {announce.Type.ToString()}, reward: {announce.Reward})");
                        ownScore += announce.Reward;
                    }
                }
                else
                {
                    foreach (Player player in players)
                    {
                        player.Prompt(
                            $"{((player.Team == this) ? "Your team" : "The opposite team")} has failed to complete an announce. (type: {announce.Type.ToString()}, reward: {announce.Reward})");
                        otherScore += announce.Reward;
                    }
                }
            }

            this.Announces.Clear();
            this.RoundScore += ownScore;
            return otherScore;
        }

        public bool HasValidatedContract()
        {
            if (this.Contract != null)
            {
                if (this.IsCapot)
                {
                    return this.TricksWon == 8;
                }

                return this.RoundScore + this.BonusRoundScore >= this.Contract.Score;
            }

            return false;
        }

        public void AwardContractPoints()
        {
            if (this.IsCapot)
            {
                this.RoundScore += 500 + this.BonusRoundScore;
            }
            else
            {
                this.RoundScore += this.Contract.Score + this.BonusRoundScore;
            }
        }

        public void AwardOppositeTeamContract(Team oppositeTeam)
        {
            if (oppositeTeam.IsCapot)
            {
                this.RoundScore += 500 + oppositeTeam.BonusRoundScore + this.BonusRoundScore;
            }
            else
            {
                this.RoundScore += oppositeTeam.Contract.Score + oppositeTeam.BonusRoundScore + this.BonusRoundScore;
            }
        }
    }
}