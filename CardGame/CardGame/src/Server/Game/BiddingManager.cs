namespace CardGame.Server.Game
{
    using System.Collections.Generic;

    using CardGame.Protocol;

    public class BiddingManager : IPhaseManager
    {
        public BiddingManager(Team[] teams)
        {
            this.Teams = teams;
            this.TurnsPassed = 0;
            this.HasEnded = false;
            this.IsContractSetup = false;
        }

        public Team[] Teams { get; }

        public IDictionary<Team, List<string>> ToPrompt { get; private set; }

        public Reply Reply { get; private set; }

        public Team CurrentWinningTeam { get; private set; }

        public bool HasEnded { get; private set; }

        public bool Success { get; private set; }

        public bool IsContractSetup { get; private set; }

        public Contract.Types.Type TrumpType { get; private set; }

        private int TurnsPassed { get; set; }

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
            this.Reply = null;
            this.Success = false;
        }

        public int HandleTurn(Player player, Event message)
        {
            this.InitTurn();

            switch (message.Type)
            {
                case Event.Types.Type.Contract:
                    this.HandleContract(message, player);
                    break;

                case Event.Types.Type.Pass:
                    this.HandlePass(player);
                    break;

                case Event.Types.Type.Coinche:
                    this.HandleCoinche(player);
                    break;

                case Event.Types.Type.Surcoinche:
                    this.HandleSurcoinche(player);
                    break;

                default:
                    this.Reply = new Reply
                    {
                        Number = 401,
                        Message = "You cannot perform this action while in the bidding phase."
                    };
                    return 0;
            }

            this.HandleEnd(player);
            return 0;
        }

        private void HandleEnd(Player player)
        {
            if (this.TurnsPassed >= 3 && this.IsContractSetup)
            {
                this.ToPrompt[player.Team].Add("End of the bidding phase.");
                this.ToPrompt[player.Team.OppositeTeam].Add("End of the bidding phase.");
                this.HasEnded = true;
                if (this.CurrentWinningTeam?.Contract != null)
                {
                    string str;
                    if (player.Team == this.CurrentWinningTeam)
                    {
                        this.TrumpType = player.Team.Contract.Type;
                        str = $"must fulfill this contract. (" + $"score: {player.Team.Contract.Score}, "
                              + $"trump: {player.Team.Contract.Type.ToString()}, "
                              + $"options:{(player.Team.IsCapot) ? " CAPOT" : string.Empty}{(player.Team.OppositeTeam.HasCoinched) ? " COINCHE" : string.Empty}{(player.Team.HasSurcoinched) ? " SURCOINCHE" : string.Empty})";
                        this.ToPrompt[player.Team].Add($"Your team {str}");
                        this.ToPrompt[player.Team.OppositeTeam].Add($"The opposite team {str}");
                    }
                    else
                    {
                        this.TrumpType = player.Team.OppositeTeam.Contract.Type;
                        str = $"must fulfill this contract. (" + $"score: {player.Team.OppositeTeam.Contract.Score}, "
                              + $"trump: {player.Team.OppositeTeam.Contract.Type.ToString()}, "
                              + $"options:{(player.Team.OppositeTeam.IsCapot) ? " CAPOT" : string.Empty}{(player.Team.HasCoinched) ? " COINCHE" : string.Empty}{(player.Team.OppositeTeam.HasSurcoinched) ? " SURCOINCHE" : string.Empty})";
                        this.ToPrompt[player.Team].Add($"The opposite team {str}");
                        this.ToPrompt[player.Team.OppositeTeam].Add($"Your team {str}");
                    }
                }
            }
        }

        private void HandleContract(Event message, Player player)
        {
            if (message.Contract == null)
            {
                this.Reply = new Reply { Number = 479, Message = "Invalid contract." };
            }
            else if (player.Team.OppositeTeam.HasCoinched)
            {
                this.Reply = new Reply { Number = 471, Message = "The opposite team has used COINCHE, you can only answer by SURCOINCHE or PASS." };
            }
            else if (player.Team.HasCoinched)
            {
                this.Reply = new Reply { Number = 472, Message = "Your team has used COINCHE, you cannot submit a contract anymore." };
            }
            else if (message.Contract.Score > 160 && message.Contract.Score != 250)
            {
                this.Reply = new Reply { Number = 473, Message = "Invalid contract, score must be lower than 160 or equal to 250 (CAPOT)." };
            }
            else if (player.Team.OppositeTeam.IsCapot)
            {
                this.Reply = new Reply { Number = 474, Message = "The opposite team is CAPOT, you must answer with COINCHE or PASS" };
            }
            else if (this.CurrentWinningTeam?.Contract != null && this.CurrentWinningTeam.Contract.Score + 10 > message.Contract.Score)
            {
                this.Reply = new Reply { Number = 475, Message = $"Invalid contract, score must be higher than the previous valid contract + 9 ({player.Team.OppositeTeam.Contract.Score + 9})" };
            }
            else
            {
                player.Team.Contract = message.Contract;
                player.Team.OppositeTeam.Contract = null;
                this.CurrentWinningTeam = player.Team;
                if (message.Contract.Score == 250)
                {
                    this.TurnsPassed = 2;
                    player.Team.IsCapot = true;
                    this.ToPrompt[player.Team].Add("Your team is now CAPOT");
                    this.ToPrompt[player.Team.OppositeTeam].Add(
                        "The opposite team is CAPOT, do you want to COINCHE ? (COINCHE/PASS)");
                }
                else
                {
                    player.Team.IsCapot = false;
                    this.TurnsPassed = 0;
                    this.ToPrompt[player.Team].Add($"Your team has made a new contract. " +
                                                   $"score: {message.Contract.Score}, " +
                                                   $"type: {message.Contract.Type.ToString()}.");
                    this.ToPrompt[player.Team].Add($"The opposite team has made a new contract. " +
                                                   $"score: {message.Contract.Score}, " +
                                                   $"type: {message.Contract.Type.ToString()}.");
                }

                this.Reply = new Reply { Number = 200, Message = "SUCCESS" };
                this.Success = true;
                this.IsContractSetup = true;
            }
        }

        private void HandlePass(Player player)
        {
            this.ToPrompt[player.Team].Add("Your team has passed");
            this.ToPrompt[player.Team.OppositeTeam].Add("The opposite team has passed");
            this.TurnsPassed++;
            this.Reply = new Reply { Number = 200, Message = "SUCCESS" };
            this.Success = true;
        }

        private void HandleCoinche(Player player)
        {
            if (player.Team.OppositeTeam.Contract == null)
            {
                this.Reply = new Reply { Number = 471, Message = "The opposite team has no valid contract for you to COINCHE" };
            }
            else if (player.Team.HasCoinched || player.Team.HasSurcoinched || player.Team.OppositeTeam.HasCoinched
                     || player.Team.OppositeTeam.HasSurcoinched)
            {
                this.Reply = new Reply { Number = 472, Message = "You cannot use COINCHE at the moment" };
            }
            else
            {
                player.Team.HasCoinched = true;
                this.ToPrompt[player.Team].Add("Your team used COINCHE");
                this.ToPrompt[player.Team.OppositeTeam].Add("The opposite team used COINCHE");
                this.Reply = new Reply { Number = 200, Message = "SUCCESS" };
                this.Success = true;
            }
        }

        private void HandleSurcoinche(Player player)
        {
            if (!player.Team.OppositeTeam.HasCoinched)
            {
                this.Reply = new Reply { Number = 481, Message = "The opposite team didn't use COINCHE" };
            }
            else
            {
                player.Team.HasSurcoinched = true;
                player.Team.OppositeTeam.HasCoinched = false;
                this.ToPrompt[player.Team].Add("Your team used SURCOINCHE");
                this.ToPrompt[player.Team.OppositeTeam].Add("The opposite team used SURCOINCHE");
                this.Reply = new Reply { Number = 200, Message = "SUCCESS" };
                this.TurnsPassed = 3;
                this.Success = true;
            }
        }
    }
}