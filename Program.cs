using System;

namespace BeatEmUpGame
{
    /// <summary>
    /// Program entry point - starts the game
    /// </summary>
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
}
