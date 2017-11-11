namespace Server.Game
{
    using System.Collections.Generic;
    using System.Linq;

    using CardGame.Protocol;

    /// <summary>
    /// The round.
    /// </summary>
    public partial class Round
    {
        public Round(Team[] teams, CardManager cardManager, int roundNumber, Game game)
        {
            this.Teams = teams;
            this.CardManager = cardManager;
            this.RoundNumber = roundNumber;
            this.State = RoundState.Bidding;
            this.Game = game;
            this.Tricks = new List<TrickManager>();
            this.BiddingManager = new BiddingManager(this.Teams);
            this.PhaseManager = this.BiddingManager;
            this.Game.PlayerManager.SetupForNewRound();

            this.CardManager.Mix();
            this.CardManager.DistributeToAll(this.Game.PlayerManager.Players);

            this.Game.PlayerManager.PromptToAll($"Round {this.RoundNumber} starting :");
            this.Game.PlayerManager.PromptToAll("Bidding phase :");
        }

        public int RoundNumber { get; private set; }

        public Team[] Teams { get; private set; }

        public CardManager CardManager { get; private set; }

        public BiddingManager BiddingManager { get; private set; }

        public Game Game { get; private set; }

        public List<TrickManager> Tricks { get; }

        public bool HasEnded { get; private set; }

        public RoundState State { get; private set; }

        private IPhaseManager PhaseManager { get; set; }

        public bool HandlePlayerMessage(Player player, Event message)
        {
            bool success;

            this.PhaseManager.HandleTurn(player, message);
            success = this.PhaseManager.Success;

            if (this.PhaseManager.Reply != null)
            {
                player.Reply(this.PhaseManager.Reply);
            }

            if (this.PhaseManager.ToPrompt != null && this.PhaseManager.ToPrompt.Any())
            {
                foreach (KeyValuePair<Team, List<string>> keyValuePair in this.PhaseManager.ToPrompt)
                {
                    foreach (string s in keyValuePair.Value)
                    {
                        keyValuePair.Key.Players[0].Prompt(s);
                        keyValuePair.Key.Players[1].Prompt(s);
                    }
                }
            }

            if (this.PhaseManager.HasEnded)
            {
                if (this.State == RoundState.Bidding)
                {
                    this.State = RoundState.Tricks;
                    this.Game.PlayerManager.PromptToAll("End of the bidding phase, game phase starting");
                }

                if (this.Tricks.Count == 8)
                {
                    this.HasEnded = true;
                    ScoreManager.CountScores(this.Game.PlayerManager, this.Teams);
                }
                else
                {
                    if (this.Tricks.Count == 0)
                    {
                        this.SetupAnnounces();
                    }
                    this.Tricks.Add(new TrickManager(this.Teams, this.CardManager, this.Tricks.Count));
                    this.PhaseManager = this.Tricks.Last();
                    this.Game.PlayerManager.PromptToAll($"Trick {this.Tricks.Count}:");
                }
            }

            return success;
        }

        private void SetupAnnounces()
        {
            Announce teamOneBestAnnounce = this.Teams[0].GetBestAnnounce();
            Announce teamTwoBestAnnounce = this.Teams[1].GetBestAnnounce();
            Team loser = null;

            if (teamOneBestAnnounce == null && teamTwoBestAnnounce != null)
            {
                loser = this.Teams[0];
            }
            else if (teamOneBestAnnounce != null && teamTwoBestAnnounce == null)
            {
                loser = this.Teams[1];
            }
            else if (teamOneBestAnnounce != null)
            {
                loser = teamOneBestAnnounce.CompareTo(teamTwoBestAnnounce) > 0 ? this.Teams[0] : this.Teams[1];
            }

            if (loser != null)
            {
                //// PROMPT
                loser.Announces.Clear();
            }
        }
    }
}