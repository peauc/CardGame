namespace Server.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CardGame.Protocol;

    /// <summary>
    /// The round.
    /// </summary>
    public partial class Round
    {
        public Round(Game game)
        {
            this.Teams = game.Teams;
            this.CardManager = game.CardManager;
            this.RoundNumber = game.CurrentRoundIndex;
            this.State = RoundState.Bidding;
            this.Game = game;
            this.Tricks = new List<TrickManager>();
            this.BiddingManager = new BiddingManager(this.Teams, this.CardManager);
            this.PhaseManager = this.BiddingManager;
            this.Game.PlayerManager.SetupForNewRound();
            this.Game.Teams[0].PrepareRound(false, false, false, null);
            this.Game.Teams[1].PrepareRound(false, false, false, null);

            this.CardManager.Mix();
            this.CardManager.DistributeToAll(this.Game.PlayerManager.Players);

            this.Game.PlayerManager.PromptToAll(string.Empty);
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
                foreach (Team team in this.Teams)
                {
                    List<string> messages = this.PhaseManager.ToPrompt[team];
                    foreach (string s in messages)
                    {
                        team.Players[0].Prompt(s);
                        team.Players[1].Prompt(s);
                    }
                }
            }

            if (this.PhaseManager.HasEnded)
            {
                if (this.State == RoundState.Bidding)
                {
                    this.State = RoundState.Tricks;
                    this.Game.PlayerManager.PromptToAll(string.Empty);
                    this.Game.PlayerManager.PromptToAll("End of the bidding phase, game phase starting.");
                    this.Game.PlayerManager.PromptToAll(string.Empty);
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

        public void SetupAnnounces()
        {
            Announce teamOneBestAnnounce = this.Teams[0].GetBestAnnounce();
            Announce teamTwoBestAnnounce = this.Teams[1].GetBestAnnounce();
            Team winner = null;

            if (teamOneBestAnnounce == null && teamTwoBestAnnounce != null)
            {
                winner = this.Teams[1];
            }
            else if (teamOneBestAnnounce != null && teamTwoBestAnnounce == null)
            {
                winner = this.Teams[0];
            }
            else if (teamOneBestAnnounce != null)
            {
                winner = teamOneBestAnnounce.CompareTo(teamTwoBestAnnounce) < 0 ? this.Teams[1] : this.Teams[0];
            }

            if (winner != null)
            {
                winner.Prompt(string.Empty);
                winner.OppositeTeam.Prompt(string.Empty);
                winner.Prompt($"Your team has won the announces phase and must now fulfill {((winner.Announces.Count != 1) ? "those announces" : "this announce")}:");
                winner.OppositeTeam.Prompt($"The opposite team has won the announces phase and must now fulfill {((winner.Announces.Count != 1) ? "those announces" : "this announce")}:");
                foreach (Announce winnerAnnounce in winner.Announces)
                {
                    winner.Prompt($"Type : {winnerAnnounce.Type}; Highest Card : {winnerAnnounce.CardsToValidate.Last().Card.Value.ToString()} of {winnerAnnounce.CardsToValidate.Last().Card.Type.ToString()}; Reward : {winnerAnnounce.Reward}");
                    winner.OppositeTeam.Prompt($"Type : {winnerAnnounce.Type}; Highest Card : {winnerAnnounce.CardsToValidate.Last().Card.Value.ToString()} of {winnerAnnounce.CardsToValidate.Last().Card.Type.ToString()}; Reward : {winnerAnnounce.Reward}");
                }

                winner.OppositeTeam.Announces.Clear();
            }
        }
    }
}