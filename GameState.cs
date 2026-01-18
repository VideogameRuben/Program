namespace BeatEmUpGame
{
    /// <summary>
    /// Enum representing the different states of the game.
    /// Used to manage game flow and determine which update/draw logic to execute.
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// Main menu state - displays title screen and options
        /// </summary>
        Menu,

        /// <summary>
        /// Active gameplay state - player is playing the game
        /// </summary>
        Playing,

        /// <summary>
        /// Paused state - gameplay is frozen, pause menu is shown
        /// </summary>
        Paused,

        /// <summary>
        /// Game over state - player has lost, displays game over screen
        /// </summary>
        GameOver
    }
}
