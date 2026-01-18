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
        
        // Jump system (separate from Y position)
        private float _jumpHeight;          // How high in the air the player is
        private float _jumpVelocity;        // Vertical jump velocity

        // Player dimensions
        private const int PLAYER_WIDTH = 32;
        private const int PLAYER_HEIGHT = 64;

        // Movement constants
        private const float MOVE_SPEED = 200f;          // Horizontal movement speed (pixels/second)
        private const float VERTICAL_MOVE_SPEED = 150f; // Vertical movement speed (pixels/second)
        private const float JUMP_VELOCITY = -500f;      // Initial jump velocity (negative = upward)
        private const float GRAVITY = 1500f;            // Gravity acceleration (pixels/second²)
        private const float MAX_FALL_SPEED = 800f;      // Terminal velocity

        // Ground level (Y position where player stands)
        private const float GROUND_LEVEL = 500f;
        
        // Vertical movement bounds (beat 'em up style)
        private const float MIN_Y = 350f;   // Top boundary (edge of brown ground area)
        private const float MAX_Y = 650f;   // Bottom boundary (bottom of screen area)

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
            _jumpHeight = 0f;
            _jumpVelocity = 0f;
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

            // Handle horizontal and vertical movement input
            HandleMovementInput(keyboardState);

            // Handle jump input with spacebar
            HandleJumpInput(keyboardState);

            // Apply jump physics (separate from ground position)
            ApplyJumpPhysics(deltaTime);

            // Update position based on velocity (only affects X and Y ground position)
            Position += _velocity * deltaTime;
            
            // Apply bounds checking for vertical movement
            ApplyBounds();
        }

        /// <summary>
        /// Handle movement input (left/right and up/down for beat 'em up style)
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
            
            // Vertical movement (beat 'em up style)
            // This allows walking up/down the playfield at any time
            // Move up (toward back of screen)
            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
            {
                _velocity.Y = -VERTICAL_MOVE_SPEED;
            }
            // Move down (toward front of screen)
            else if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
            {
                _velocity.Y = VERTICAL_MOVE_SPEED;
            }
            else
            {
                _velocity.Y = 0; // No vertical movement if not pressing up/down
            }
        }

        /// <summary>
        /// Handle jump input (only allow jumping when on ground)
        /// </summary>
        private void HandleJumpInput(KeyboardState keyboardState)
        {
            // Jump with Space key, but only if on ground (jump height is 0)
            if (keyboardState.IsKeyDown(Keys.Space) && _jumpHeight <= 0f)
            {
                _jumpVelocity = JUMP_VELOCITY; // Start jumping up
                _isOnGround = false;
            }
        }

        /// <summary>
        /// Apply jump physics: gravity affects jump height, not Y position
        /// </summary>
        private void ApplyJumpPhysics(float deltaTime)
        {
            // Only apply jump physics if we have jump velocity or are in the air
            if (_jumpHeight > 0f || _jumpVelocity < 0f)
            {
                // Apply gravity to jump velocity (makes it more positive = falling)
                _jumpVelocity += GRAVITY * deltaTime;
                
                // Clamp fall speed
                if (_jumpVelocity > MAX_FALL_SPEED)
                    _jumpVelocity = MAX_FALL_SPEED;
                
                // Update jump height based on velocity
                // Negative velocity = going up, positive = falling down
                _jumpHeight -= _jumpVelocity * deltaTime;
                
                // Check if landed on ground
                if (_jumpHeight <= 0f)
                {
                    _jumpHeight = 0f;
                    _jumpVelocity = 0f;
                    _isOnGround = true;
                }
                else
                {
                    _isOnGround = false;
                }
            }
            else
            {
                // On ground
                _isOnGround = true;
            }
        }
        
        /// <summary>
        /// Keep player within vertical bounds (beat 'em up style)
        /// Prevents walking above the top edge of the ground or below bottom of screen
        /// </summary>
        private void ApplyBounds()
        {
            // Clamp Y position between min and max bounds
            float newY = Position.Y;
            
            // Can't walk above the top boundary (edge of brown ground)
            if (newY < MIN_Y)
            {
                newY = MIN_Y;
                _velocity.Y = 0;
            }
            
            // Can't walk below the bottom boundary (edge of screen)
            if (newY > MAX_Y)
            {
                newY = MAX_Y;
                _velocity.Y = 0;
            }
            
            Position = new Vector2(Position.X, newY);
        }

        /// <summary>
        /// Draw the player sprite with shadow
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch for rendering</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_texture == null)
                return;

            // Draw shadow (ellipse at player's feet)
            DrawShadow(spriteBatch);

            // Create a rectangle for the player's sprite
            // Jump height affects visual position (raises sprite up)
            Rectangle destinationRect = new Rectangle(
                (int)Position.X,
                (int)(Position.Y - _jumpHeight), // Subtract jump height to raise sprite
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
        /// Draw shadow underneath the player
        /// </summary>
        private void DrawShadow(SpriteBatch spriteBatch)
        {
            // Shadow is an ellipse at the player's feet
            int shadowWidth = 40;
            int shadowHeight = 10;
            
            // Shadow appears at ground position (Y coordinate)
            Rectangle shadowRect = new Rectangle(
                (int)Position.X + (PLAYER_WIDTH - shadowWidth) / 2,
                (int)Position.Y + PLAYER_HEIGHT - 5,
                shadowWidth,
                shadowHeight
            );
            
            // Draw semi-transparent black shadow
            spriteBatch.Draw(
                texture: _texture,
                destinationRectangle: shadowRect,
                sourceRectangle: null,
                color: Color.Black * 0.3f,
                rotation: 0f,
                origin: Vector2.Zero,
                effects: SpriteEffects.None,
                layerDepth: 0f
            );
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
            _jumpHeight = 0f;
            _jumpVelocity = 0f;
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
