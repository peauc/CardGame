using System;
namespace CardGame.Server.Game
{
    public partial class Game
    {
        public enum GameState {
            AWAITING_PLAYERS,
            BIDDING,
            GAME,
            SCORES,
        }
    }
}
