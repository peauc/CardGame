﻿namespace Server.Game
{
    using System;

    public static class ScoreManager
    {
        public static void CountScores(PlayerManager playerManager, Team[] teams)
        {
            Team takers;
            Team passers;
            int bonus = 0;
            int takersScoreMultiplier = 1;

            if (teams[0].Contract != null)
            {
                takers = teams[0];
                passers = teams[1];
            }
            else
            {
                takers = teams[1];
                passers = teams[2];
            }

            if (passers.HasCoinched)
            {
                takersScoreMultiplier = 2;
            }
            else if (takers.HasSurcoinched)
            {
                takersScoreMultiplier = 4;
            }

            if (takers.HasValidatedContract() && takers.RoundScore + takers.BonusRoundScore > passers.RoundScore)
            {
                takers.AwardContractPoints(takersScoreMultiplier);
                passers.RoundScore += passers.BonusRoundScore;
                playerManager.PromptToAll("The contract has been fulfilled.");
            }
            else
            {
                passers.AwardOppositeTeamContract(takers, takersScoreMultiplier);
                playerManager.PromptToAll("The contract has not been fulfilled.");
            }

            bonus = teams[0].ValidateAnnounces(
                teams[1].ValidateAnnounces(bonus, playerManager.Players),
                playerManager.Players);

            teams[1].BonusRoundScore += bonus;

            takers.TotalScore += takers.RoundScore + takers.BonusRoundScore;
            passers.TotalScore += passers.RoundScore + passers.BonusRoundScore;
            playerManager.PromptToAll("Team 1 now has " + teams[0].TotalScore);
            playerManager.PromptToAll("Team 2 now has " + teams[1].TotalScore);
        }

        public static void ShowScores(Team[] teams, PlayerManager playerManager)
        {
            if (teams[0].TotalScore > 3000 && teams[0].TotalScore > teams[1].TotalScore)
            {
                playerManager.PromptToAll($"Team 1 has won with {teams[0].TotalScore} points !");
            }
            else
            {
                playerManager.PromptToAll($"Team 2 has won with {teams[1].TotalScore} points !");
            }
        }
    }
}