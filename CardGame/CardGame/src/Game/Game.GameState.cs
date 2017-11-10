namespace Server.Game
{
    /// <summary>
    /// The game.
    /// </summary>
    public partial class Game
    {
        public enum GameState
        {
            AwaitingPlayers,
            Bidding,
            Game,
            Scores
        }
    }
}