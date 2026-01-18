using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BeatEmUpGame
{
    /// <summary>
    /// Player class representing the playable character.
    /// Handles player movement, physics (gravity, jumping), input, and rendering.
    /// </summary>
    public class Player
    {
        // Position and movement
        public Vector2 Position { get; private set; }
        private Vector2 _velocity;

        // Player dimensions
        private const int PLAYER_WIDTH = 32;
        private const int PLAYER_HEIGHT = 64;

        // Movement constants
        private const float MOVE_SPEED = 200f;          // Horizontal movement speed (pixels/second)
        private const float JUMP_VELOCITY = -500f;      // Initial jump velocity (negative = upward)
        private const float GRAVITY = 1500f;            // Gravity acceleration (pixels/second²)
        private const float MAX_FALL_SPEED = 800f;      // Terminal velocity

        // Ground level (Y position where player stands)
        private const float GROUND_LEVEL = 500f;

        // Player state
        private bool _isOnGround;
        private bool _isFacingRight;

        // Rendering
        private Texture2D _texture;
        private Color _color;

        /// <summary>
        /// Constructor: Initialize player at a starting position
        /// </summary>
        /// <param name="startPosition">Initial position in world space</param>
        public Player(Vector2 startPosition)
        {
            Position = startPosition;
            _velocity = Vector2.Zero;
            _isOnGround = false;
            _isFacingRight = true;
            _color = Color.Orange; // Default color for placeholder
        }

        /// <summary>
        /// Set the player's texture (called after content loading)
        /// </summary>
        public void SetTexture(Texture2D texture)
        {
            _texture = texture;
        }

        /// <summary>
        /// Update player logic: input handling and physics
        /// </summary>
        /// <param name="gameTime">Timing information</param>
        /// <param name="keyboardState">Current keyboard input state</param>
        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Handle horizontal movement input
            HandleMovementInput(keyboardState);

            // Handle jump input
            HandleJumpInput(keyboardState);

            // Apply physics (gravity and velocity)
            ApplyPhysics(deltaTime);

            // Update position based on velocity
            Position += _velocity * deltaTime;

            // Check ground collision
            CheckGroundCollision();

            // TODO: Add bounds checking to keep player within level boundaries
        }

        /// <summary>
        /// Handle left/right movement input
        /// </summary>
        private void HandleMovementInput(KeyboardState keyboardState)
        {
            _velocity.X = 0; // Reset horizontal velocity

            // Move left
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                _velocity.X = -MOVE_SPEED;
                _isFacingRight = false;
            }

            // Move right
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                _velocity.X = MOVE_SPEED;
                _isFacingRight = true;
            }
        }

        /// <summary>
        /// Handle jump input (only allow jumping when on ground)
        /// </summary>
        private void HandleJumpInput(KeyboardState keyboardState)
        {
            // Jump with Space or W key, but only if on ground
            if ((keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.W)) && _isOnGround)
            {
                _velocity.Y = JUMP_VELOCITY;
                _isOnGround = false;
            }
        }

        /// <summary>
        /// Apply physics: gravity and velocity clamping
        /// </summary>
        private void ApplyPhysics(float deltaTime)
        {
            // Apply gravity (only when in air)
            if (!_isOnGround)
            {
                _velocity.Y += GRAVITY * deltaTime;

                // Clamp to maximum fall speed
                if (_velocity.Y > MAX_FALL_SPEED)
                    _velocity.Y = MAX_FALL_SPEED;
            }
        }

        /// <summary>
        /// Check if player has landed on the ground
        /// </summary>
        private void CheckGroundCollision()
        {
            // Simple ground collision: check if player is at or below ground level
            if (Position.Y >= GROUND_LEVEL)
            {
                Position = new Vector2(Position.X, GROUND_LEVEL);
                _velocity.Y = 0;
                _isOnGround = true;
            }
            else
            {
                _isOnGround = false;
            }
        }

        /// <summary>
        /// Draw the player sprite
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch for rendering</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_texture == null)
                return;

            // Create a rectangle for the player's sprite
            Rectangle destinationRect = new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                PLAYER_WIDTH,
                PLAYER_HEIGHT
            );

            // Flip sprite horizontally based on facing direction
            SpriteEffects spriteEffect = _isFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            // Draw the player sprite
            spriteBatch.Draw(
                texture: _texture,
                destinationRectangle: destinationRect,
                sourceRectangle: null, // Use entire texture (for sprite sheet, specify source rectangle)
                color: _color,
                rotation: 0f,
                origin: Vector2.Zero,
                effects: spriteEffect,
                layerDepth: 0f
            );

            // TODO: When using sprite sheets, animate based on player state (idle, walk, jump, attack)
        }

        /// <summary>
        /// Reset player to a starting position (used for game restart)
        /// </summary>
        /// <param name="startPosition">Position to reset to</param>
        public void Reset(Vector2 startPosition)
        {
            Position = startPosition;
            _velocity = Vector2.Zero;
            _isOnGround = false;
            _isFacingRight = true;
        }

        /// <summary>
        /// Get the player's bounding box for collision detection
        /// </summary>
        public Rectangle GetBoundingBox()
        {
            return new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                PLAYER_WIDTH,
                PLAYER_HEIGHT
            );
        }
    }
}
