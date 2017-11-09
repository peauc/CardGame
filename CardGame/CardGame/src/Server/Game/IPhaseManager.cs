namespace CardGame.Server.Game
{
    using System.Collections.Generic;

    using CardGame.Protocol;

    public interface IPhaseManager
    {
        bool HasEnded { get; }

        Reply Reply { get; }

        bool Success { get; }

        Team[] Teams { get; }

        IDictionary<Team, List<string>> ToPrompt { get; }

        void InitTurn();

        int HandleTurn(Player player, Event message);
    }
}