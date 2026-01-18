using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeatEmUpGame
{
    /// <summary>
    /// Represents a game level with boundaries, background, and completion detection
    /// </summary>
    public class Level
    {
        // Level boundaries
        public float StartX { get; private set; }
        public float EndX { get; private set; }
        public float Width => EndX - StartX;

        // Level visual properties
        private Color _backgroundColor;
        private Color _groundColor;
        
        // Ground properties
        private const float GROUND_HEIGHT = 100f;
        
        // Completion tracking
        public bool IsComplete { get; private set; }

        /// <summary>
        /// Create a level with specified boundaries
        /// </summary>
        /// <param name="startX">Starting X position</param>
        /// <param name="endX">Ending X position (mission complete point)</param>
        public Level(float startX, float endX)
        {
            StartX = startX;
            EndX = endX;
            IsComplete = false;
            
            // Visual settings
            _backgroundColor = new Color(135, 206, 235); // Sky blue
            _groundColor = new Color(139, 69, 19); // Brown ground
        }

        /// <summary>
        /// Check if player has reached the end of the level
        /// </summary>
        public void CheckCompletion(Vector2 playerPosition)
        {
            if (playerPosition.X >= EndX - 50) // Give 50px margin
            {
                IsComplete = true;
            }
        }

        /// <summary>
        /// Reset level completion state
        /// </summary>
        public void Reset()
        {
            IsComplete = false;
        }

        /// <summary>
        /// Draw the level background
        /// Simple version with colored rectangles - can be upgraded to tiled backgrounds later
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Texture2D pixel, Rectangle visibleArea)
        {
            // Draw sky background (fills visible area)
            spriteBatch.Draw(pixel, visibleArea, _backgroundColor);

            // Draw ground
            Rectangle groundRect = new Rectangle(
                (int)StartX,
                500, // Ground Y position (matches player GROUND_LEVEL)
                (int)Width,
                1000 // Very tall to fill bottom of screen
            );
            spriteBatch.Draw(pixel, groundRect, _groundColor);

            // Optional: Draw end zone marker (visual indicator)
            Rectangle endZoneRect = new Rectangle(
                (int)EndX - 100,
                0,
                100,
                720
            );
            spriteBatch.Draw(pixel, endZoneRect, Color.Gold * 0.3f); // Semi-transparent gold
        }

        /// <summary>
        /// Get the starting position for the player in this level
        /// </summary>
        public Vector2 GetPlayerStartPosition()
        {
            return new Vector2(StartX + 100, 400);
        }
    }
}
