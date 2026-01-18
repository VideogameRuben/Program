using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeatEmUpGame
{
    /// <summary>
    /// Camera class that creates a viewport transformation for 2D side-scrolling.
    /// Follows the player horizontally while keeping the view centered.
    /// </summary>
    public class Camera
    {
        // Camera position in world space
        private Vector2 _position;

        // Viewport for camera bounds
        private Viewport _viewport;

        // Camera smoothing/following settings
        private const float FOLLOW_SPEED = 0.1f; // Lower = smoother, slower camera
        private const float MIN_X = 0f;          // Minimum X position (left boundary)
        private const float MAX_X = 5000f;       // Maximum X position (right boundary of level)

        /// <summary>
        /// Get the camera's current position
        /// </summary>
        public Vector2 Position => _position;

        /// <summary>
        /// Constructor: Initialize the camera with viewport information
        /// </summary>
        /// <param name="viewport">The game's viewport</param>
        public Camera(Viewport viewport)
        {
            _viewport = viewport;
            _position = Vector2.Zero;
        }

        /// <summary>
        /// Update camera to follow a target position (typically the player).
        /// The camera smoothly moves to keep the target centered horizontally.
        /// Vertical position is fixed (no vertical scrolling).
        /// </summary>
        /// <param name="targetPosition">Position to follow (usually player position)</param>
        public void Follow(Vector2 targetPosition)
        {
            // Calculate the ideal camera position (centered on target)
            // We want the target to appear in the center of the screen
            float idealX = targetPosition.X - (_viewport.Width / 2);

            // Clamp camera position to level boundaries
            idealX = MathHelper.Clamp(idealX, MIN_X, MAX_X);

            // Smooth camera movement (lerp from current to ideal position)
            _position.X = MathHelper.Lerp(_position.X, idealX, FOLLOW_SPEED);

            // Keep Y position fixed (no vertical scrolling for beat 'em up style)
            // You could adjust this to follow the player vertically if needed
            _position.Y = 0;
        }

        /// <summary>
        /// Get the transformation matrix for the camera.
        /// This matrix is applied to the SpriteBatch to transform world coordinates
        /// to screen coordinates, creating the scrolling effect.
        /// </summary>
        /// <returns>Transformation matrix for rendering</returns>
        public Matrix GetTransformMatrix()
        {
            // Create a translation matrix that moves everything by -camera position
            // This makes objects in the world appear to move relative to the camera
            return Matrix.CreateTranslation(
                -_position.X,  // Move horizontally based on camera position
                -_position.Y,  // Move vertically (usually 0 for beat 'em up)
                0              // Z-axis (not used in 2D)
            );
        }

        /// <summary>
        /// Convert screen coordinates to world coordinates.
        /// Useful for mouse input or UI positioning.
        /// </summary>
        /// <param name="screenPosition">Position on screen</param>
        /// <returns>Position in world space</returns>
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return screenPosition + _position;
        }

        /// <summary>
        /// Convert world coordinates to screen coordinates.
        /// Useful for UI elements that need to align with world objects.
        /// </summary>
        /// <param name="worldPosition">Position in world space</param>
        /// <returns>Position on screen</returns>
        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return worldPosition - _position;
        }

        /// <summary>
        /// Reset camera to initial position
        /// </summary>
        public void Reset()
        {
            _position = Vector2.Zero;
        }

        /// <summary>
        /// Immediately set camera position (no smoothing)
        /// </summary>
        /// <param name="position">New camera position</param>
        public void SetPosition(Vector2 position)
        {
            _position = position;
        }

        /// <summary>
        /// Get the visible area of the game world (rectangle in world space)
        /// Useful for culling objects outside the camera view
        /// </summary>
        /// <returns>Rectangle representing visible area</returns>
        public Rectangle GetVisibleArea()
        {
            return new Rectangle(
                (int)_position.X,
                (int)_position.Y,
                _viewport.Width,
                _viewport.Height
            );
        }
    }
}
