namespace Server.Game
{
    using System.Collections.Generic;
    using System.Linq;

    using CardGame.Protocol;

    public class TrickManager : IPhaseManager
    {
        public TrickManager(Team[] teams, CardManager cardManager, int round)
        {
            this.Teams = teams;
            this.Round = round;
            this.CardManager = cardManager;
            this.HasEnded = false;
        }

        public bool HasEnded { get; private set; }

        public Reply Reply { get; private set; }

        public bool Success { get; private set; }

        public Team[] Teams { get; private set; }

        public IDictionary<Team, List<string>> ToPrompt { get; private set; }

        public Card.Types.Type Entame { get; private set; }

        private int Turn { get; set; }

        private int Round { get; set; }

        private CardManager CardManager { get; set; }

        private Player Winner { get; set; }

        private IDictionary<Player, Card> CardsPlayed { get; set; }

        public void InitTurn()
        {
            this.ToPrompt = new Dictionary<Team, List<string>>
                                {
                                    {
                                        this.Teams[0],
                                        new List<string>()
                                    },
                                    {
                                        this.Teams[1],
                                        new List<string>()
                                    }
                                };
            this.CardsPlayed = new Dictionary<Player, Card>();
            this.Reply = null;
            this.Success = false;
        }

        public int HandleTurn(Player player, Event message)
        {
            this.InitTurn();

            switch (message.Type)
            {
                case Event.Types.Type.Play:
                    this.HandlePlay(message, player);
                    break;

                case Event.Types.Type.Announce:
                    this.HandleAnnounce(message, player);
                    break;

                case Event.Types.Type.Belote:
                    this.HandleBelote(player);
                    break;

                case Event.Types.Type.Rebelote:
                    this.HandleRebelote(player);
                    break;

                default:
                    this.Reply = new Reply { Number = 401, Message = "You cannot perform this action while in the game phase." };
                    return 0;
            }

            if (this.Turn == 4)
            {
                this.HandleEnd();
            }

            return 0;
        }

        private void HandleEnd()
        {
            this.Winner = this.CardManager.GetCurrentTurnWinner(this.CardsPlayed);
            Card highestCard = this.CardsPlayed[this.Winner];

            this.HasEnded = true;
            this.Winner.Team.TricksWon++;
            this.Winner.Team.RoundScore += this.CardManager.GetCardScore(highestCard) + ((this.Round == 7) ? 10 : 0);
            this.ToPrompt[this.Winner.Team].Add($"{this.Winner} has won the trick and earned {this.CardManager.GetCardScore(highestCard) + ((this.Round == 7) ? 10 : 0)}");
            this.ToPrompt[this.Winner.Team.OppositeTeam].Add($"{this.Winner} has won the trick and earned {this.CardManager.GetCardScore(highestCard) + ((this.Round == 7) ? 10 : 0)}");
        }

        private void HandlePlay(Event message, Player player)
        {
            Player ally = player.Team.Players[0] == player ? player.Team.Players[1] : player.Team.Players[0];
            if (!player.HasCard(message.Card))
            {
                this.Reply = new Reply { Number = 463, Message = "You don't have this card in your hand." };
                return;
            }

            if (this.Turn != 0)
            {
                if (message.Card.Type == this.Entame)
                {
                    this.ValidateCard(message, player);
                    return;
                }

                bool hasHigherTrumpInHand = false;
                bool hasEntameInHand = player.HasStartCard(this.Entame);
                bool trumpHasBeenPlayed = this.TrumpHasBeenPlayed();
                bool hasTrumpInHand = player.HasTrumpCard(this.CardManager.CurrentTrump);
                Player currentWinner = this.CardManager.GetCurrentTurnWinner(this.CardsPlayed);
                if (hasTrumpInHand && currentWinner != null)
                {
                    hasHigherTrumpInHand = player.HasHigherTrumpCard(this.CardManager, this.CardsPlayed[currentWinner]);
                }

                if (hasEntameInHand)
                {
                    this.Reply = new Reply { Number = 442, Message = "You must always play a card that corresponds to the first card played if you have one." };
                }
                else
                {
                    if (!hasTrumpInHand)
                    {
                        this.ValidateCard(message, player);
                    }
                    else if (trumpHasBeenPlayed && hasHigherTrumpInHand && this.CardManager.CompareCards(
                            message.Card,
                            this.CardsPlayed[currentWinner]) < 0)
                    {
                        this.Reply = new Reply { Number = 442, Message = "You have to play a trump higher than the ones on the table." };
                    }
                    else
                    {
                        if (ally == currentWinner || this.CardManager.CurrentTrump.ToString() == message.Card.Type.ToString())
                        {
                            this.ValidateCard(message, player);
                        }
                        else
                        {
                            this.Reply = new Reply { Number = 443, Message = "You have to play a trump card." };
                        }
                    }
                }
            }
            else
            {
                this.Entame = message.Card.Type;
                this.ValidateCard(message, player);
            }
        }

        private void HandleAnnounce(Event message, Player player)
        {
            if (this.Round != 0)
            {
                this.Reply = new Reply { Number = 461, Message = "You can only make an ANNOUNCE at trick 1." };
            }
            else if (message.Announce?.Card == null)
            {
                this.Reply = new Reply { Number = 462, Message = "Invalid ANNOUNCE." };
            }
            else if (!player.HasCard(message.Card))
            {
                this.Reply = new Reply { Number = 463, Message = "You don't have this card in your hand." };
            }
            else
            {
                Announce announce;

                switch (message.Announce.Type)
                {
                    case CardGame.Protocol.Announce.Types.Type.Carre:
                        announce = new Announce(player, AnnounceType.Carre, message.Announce.Card);
                        break;

                    case CardGame.Protocol.Announce.Types.Type.Cent:
                        announce = new Announce(player, AnnounceType.Cent, message.Announce.Card);
                        break;

                    case CardGame.Protocol.Announce.Types.Type.Cinquante:
                        announce = new Announce(player, AnnounceType.Cinquante, message.Announce.Card);
                        break;

                    case CardGame.Protocol.Announce.Types.Type.Tierce:
                        announce = new Announce(player, AnnounceType.Tierce, message.Announce.Card);
                        break;

                    default:
                        this.Reply = new Reply { Number = 462, Message = "Invalid ANNOUNCE." };
                        return;
                }

                player.Team.Announces.Add(announce);
                this.ToPrompt[player.Team].Add($"{player.Name} has made an ANNOUNCE of strength {announce.Reward}");
                this.ToPrompt[player.Team.OppositeTeam].Add($"{player.Name} has made an ANNOUNCE of strength {announce.Reward}");
                this.Reply = new Reply { Number = 200, Message = "SUCCESS" };
            }
        }

        private void HandleBelote(Player player)
        {
            if (player.Belote == Player.BeloteState.Declared)
            {
                this.Reply = new Reply { Number = 451, Message = "You already announced a BELOTE" };
            }
            else
            {
                this.ToPrompt[player.Team].Add($"{player.Name} has announced a BELOTE");
                this.ToPrompt[player.Team.OppositeTeam].Add($"{player.Name} has announced a BELOTE");
                player.Belote = Player.BeloteState.Declared;
                this.Reply = new Reply { Number = 200, Message = "SUCCESS" };
            }
        }

        private void HandleRebelote(Player player)
        {
            if (player.Belote == Player.BeloteState.Declared)
            {
                this.Reply = new Reply { Number = 452, Message = "To announce a REBELOTE you must first announce a BELOTE" };
            }
            else
            {
                this.ToPrompt[player.Team].Add($"{player.Name} has announced a REBELOTE");
                this.ToPrompt[player.Team.OppositeTeam].Add($"{player.Name} has announced a REBELOTE");
                player.Belote = Player.BeloteState.Declared;
                this.Reply = new Reply { Number = 200, Message = "SUCCESS" };
            }
        }

        private void CheckForBelote(Event message, Player player)
        {
            if (player.Belote == Player.BeloteState.Declared)
            {
                if (this.CardManager.IsTrumpQueen(message.Card))
                {
                    player.Belote = Player.BeloteState.Done;
                    this.ToPrompt[player.Team].Add($"{player.Name} has completed his BELOTE");
                    this.ToPrompt[player.Team.OppositeTeam].Add($"{player.Name} has completed his BELOTE");
                }
                else
                {
                    player.Belote = Player.BeloteState.Undeclared;
                    player.Team.OppositeTeam.BonusRoundScore += 20;
                    this.ToPrompt[player.Team].Add($"{player.Name} has failed to complete his BELOTE, the opposite team will be awarded a 20 point bonus.");
                    this.ToPrompt[player.Team.OppositeTeam].Add($"{player.Name} has FAILED to complete his BELOTE, your team will be awarded a 20 point bonus.");
                }
            }
        }

        private void CheckForRebelote(Event message, Player player)
        {
            if (player.Rebelote == Player.BeloteState.Declared)
            {
                if (this.CardManager.IsTrumpQueen(message.Card))
                {
                    player.Rebelote = Player.BeloteState.Done;
                    this.ToPrompt[player.Team].Add($"{player.Name} has completed his REBELOTE, your team will be awarded a 20 point bonus.");
                    this.ToPrompt[player.Team.OppositeTeam].Add($"{player.Name} has completed his REBELOTE,the opposite team will be awarded a 20 point bonus.");
                }
                else
                {
                    player.Rebelote = Player.BeloteState.Undeclared;
                    player.Team.OppositeTeam.BonusRoundScore += 20;
                    this.ToPrompt[player.Team].Add($"{player.Name} has failed to complete his REBELOTE, the opposite team will be awarded a 20 point bonus.");
                    this.ToPrompt[player.Team.OppositeTeam].Add($"{player.Name} has FAILED to complete his REBELOTE, your team will be awarded a 20 point bonus.");
                }
            }
        }

        private void ValidateCard(Event message, Player player)
        {
            this.CheckForBelote(message, player);
            this.CheckForRebelote(message, player);

            this.Turn++;
            this.CardsPlayed[player] = message.Card;
            this.ToPrompt[player.Team].Add($"{player.Name} has just played a {message.Card.Value.ToString()} of {message.Card.Type.ToString()}.");
            this.ToPrompt[player.Team.OppositeTeam].Add($"{player.Name} has just played a {message.Card.Value.ToString()} of {message.Card.Type.ToString()}.");
            this.Success = true;
            player.RemoveFromHand(message.Card);
            if (player.Team.Announces != null && player.Team.Announces.Any())
            {
                if (player.Team.Announces.Any(announce => announce.Validate(message.Card)))
                {
                }
            }
        }

        private bool TrumpHasBeenPlayed()
        {
            return this.CardsPlayed.Any(item => this.CardManager.CurrentTrump.ToString() == item.Value.Value.ToString());
        }
    }
}