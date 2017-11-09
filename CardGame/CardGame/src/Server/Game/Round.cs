namespace CardGame.Server.Game
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
            this.BiddingManager = new BiddingManager(this.Teams);
            this.PhaseManager = this.BiddingManager;

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

        public List<TrickManager> Tricks { get; private set; }

        public TrickManager CurrentTrick { get; private set; }

        public bool HasEnded { get; private set; }

        public RoundState State { get; private set; }

        private IPhaseManager PhaseManager { get; set; }

        public bool HandlePlayerMessage(Player player, Event message)
        {
            bool success;

            this.PhaseManager.HandleTurn(player, message);
            success = this.PhaseManager.Success;

            if (this.PhaseManager.HasEnded)
            {
                if (this.State == RoundState.Bidding)
                {
                    this.State = RoundState.Tricks;
                }
                if (this.Tricks.Count == 8)
                {
                    this.HasEnded = true;
                }
                else
                {
                    this.Tricks.Add(new TrickManager(this.Teams, this.CardManager, this.Tricks.Count));
                    this.PhaseManager = this.Tricks.Last();
                    this.Game.PlayerManager.PromptToAll($"Trick {this.Tricks.Count}:");
                }
            }
            return success;
        }
    }
}