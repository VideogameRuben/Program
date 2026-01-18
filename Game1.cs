using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BeatEmUpGame
{
    /// <summary>
    /// Main game class that manages the game loop, rendering, and core systems.
    /// This is the entry point for MonoGame and handles initialization, updates, and drawing.
    /// </summary>
    public class Game1 : Game
    {
        // Graphics and rendering
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _gameFont;

        // Game state management
        private GameState _currentState;

        // Game objects
        private Player _player;
        private Camera _camera;
        private Level _currentLevel;

        // Mission complete state
        private bool _missionComplete;

        // Input state tracking (to detect key press once)
        private KeyboardState _previousKeyboardState;

        // Pixel texture for drawing backgrounds
        private Texture2D _pixelTexture;

        // Screen dimensions
        private const int SCREEN_WIDTH = 1280;
        private const int SCREEN_HEIGHT = 720;

        /// <summary>
        /// Constructor: Sets up the graphics device and content root directory
        /// </summary>
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Set the screen resolution
            _graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
        }

        /// <summary>
        /// Initialize: Called once at the start before content loading.
        /// Use this to initialize game objects and set up non-content-dependent systems.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize game state - start directly in playing mode
            _currentState = GameState.Playing;

            // Initialize the camera
            _camera = new Camera(GraphicsDevice.Viewport);

            // Initialize level (0 to 3000 pixels wide)
            _currentLevel = new Level(0, 3000);

            // Initialize player at level start position
            _player = new Player(_currentLevel.GetPlayerStartPosition());

            _missionComplete = false;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent: Called once after Initialize().
        /// Load all game assets (sprites, sounds, fonts) here using the Content pipeline.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load font for UI text
            _gameFont = Content.Load<SpriteFont>("GameFont");

            // Create pixel texture for backgrounds and player
            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            // TODO: Load sprite sheets and textures here
            // Example: _player.LoadContent(Content);
            _player.SetTexture(_pixelTexture);
        }

        /// <summary>
        /// Update: Called every frame (typically 60 times per second).
        /// This is the main game logic loop where we:
        /// 1. Read input
        /// 2. Update game state
        /// 3. Update physics
        /// 4. Handle collisions
        /// 5. Update AI
        /// </summary>
        /// <param name="gameTime">Provides timing information</param>
        protected override void Update(GameTime gameTime)
        {
            // Get current input state
            KeyboardState keyboardState = Keyboard.GetState();

            // Allow exiting the game with Escape key
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            // Handle state transitions
            HandleStateTransitions(keyboardState);

            // Update based on current game state
            switch (_currentState)
            {
                case GameState.Menu:
                    UpdateMenu(keyboardState);
                    break;

                case GameState.Playing:
                    UpdatePlaying(gameTime, keyboardState);
                    break;

                case GameState.Paused:
                    UpdatePaused(keyboardState);
                    break;

                case GameState.GameOver:
                    UpdateGameOver(keyboardState);
                    break;
            }

            // Store keyboard state for next frame (to detect single key presses)
            _previousKeyboardState = keyboardState;

            base.Update(gameTime);
        }

        /// <summary>
        /// Handle transitions between game states based on input
        /// </summary>
        private void HandleStateTransitions(KeyboardState keyboardState)
        {
            // Check for pause toggle (P key)
            if (keyboardState.IsKeyDown(Keys.P) && _previousKeyboardState.IsKeyUp(Keys.P))
            {
                if (_currentState == GameState.Playing)
                    _currentState = GameState.Paused;
                else if (_currentState == GameState.Paused)
                    _currentState = GameState.Playing;
            }
        }

        /// <summary>
        /// Update logic for Menu state
        /// </summary>
        private void UpdateMenu(KeyboardState keyboardState)
        {
            // Press Enter to start the game
            if (keyboardState.IsKeyDown(Keys.Enter) && _previousKeyboardState.IsKeyUp(Keys.Enter))
            {
                _currentState = GameState.Playing;
            }
        }

        /// <summary>
        /// Update logic for Playing state - main gameplay loop
        /// </summary>
        private void UpdatePlaying(GameTime gameTime, KeyboardState keyboardState)
        {
            // Update player (handles input and physics)
            _player.Update(gameTime, keyboardState);

            // Update camera to follow player
            _camera.Follow(_player.Position);

            // Check if player reached end of level
            _currentLevel.CheckCompletion(_player.Position);
            if (_currentLevel.IsComplete && !_missionComplete)
            {
                _missionComplete = true;
            }

            // TODO: Update enemies, check collisions, etc.
        }

        /// <summary>
        /// Update logic for Paused state
        /// </summary>
        private void UpdatePaused(KeyboardState keyboardState)
        {
            // Paused - no game logic updates
            // State transition is handled in HandleStateTransitions
        }

        /// <summary>
        /// Update logic for GameOver state
        /// </summary>
        private void UpdateGameOver(KeyboardState keyboardState)
        {
            // Press Enter to return to menu
            if (keyboardState.IsKeyDown(Keys.Enter) && _previousKeyboardState.IsKeyUp(Keys.Enter))
            {
                // Reset game and return to menu
                _currentLevel.Reset();
                _missionComplete = false;
                _player.Reset(_currentLevel.GetPlayerStartPosition());
                _camera.Reset();
                _currentState = GameState.Menu;
            }
        }

        /// <summary>
        /// Draw: Called every frame after Update().
        /// This is the rendering pipeline where we draw everything to the screen:
        /// 1. Clear the screen
        /// 2. Begin SpriteBatch with camera transform
        /// 3. Draw all game objects (background, entities, UI)
        /// 4. End SpriteBatch
        /// </summary>
        /// <param name="gameTime">Provides timing information</param>
        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen with a background color
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw based on current game state
            switch (_currentState)
            {
                case GameState.Menu:
                    DrawMenu();
                    break;

                case GameState.Playing:
                case GameState.Paused:
                    DrawPlaying();
                    if (_currentState == GameState.Paused)
                        DrawPausedOverlay();
                    break;

                case GameState.GameOver:
                    DrawGameOver();
                    break;
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Draw the menu screen
        /// </summary>
        private void DrawMenu()
        {
            _spriteBatch.Begin();

            // TODO: Draw menu graphics and text
            // For now, just a simple instruction (would need a SpriteFont to draw text)

            _spriteBatch.End();
        }

        /// <summary>
        /// Draw the main gameplay screen with camera transform
        /// </summary>
        private void DrawPlaying()
        {
            // Begin drawing with camera transformation matrix
            // This allows the camera to scroll the world
            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp, // Pixel-perfect rendering for retro look
                transformMatrix: _camera.GetTransformMatrix()
            );

            // Draw level background
            _currentLevel.Draw(_spriteBatch, _pixelTexture, _camera.GetVisibleArea());

            // Draw player
            _player.Draw(_spriteBatch);

            // TODO: Draw enemies, items, effects

            _spriteBatch.End();

            // Draw UI elements (health bars, score) without camera transform
            _spriteBatch.Begin();
            
            // Draw mission complete message
            if (_missionComplete)
            {
                string completeText = "MISSION COMPLETE!";
                Vector2 textSize = _gameFont.MeasureString(completeText);
                Vector2 textPosition = new Vector2(
                    (SCREEN_WIDTH - textSize.X) / 2,
                    50
                );
                // Draw shadow
                _spriteBatch.DrawString(_gameFont, completeText, textPosition + new Vector2(2, 2), Color.Black);
                // Draw text
                _spriteBatch.DrawString(_gameFont, completeText, textPosition, Color.Gold);
            }
            
            _spriteBatch.End();
        }

        /// <summary>
        /// Draw pause overlay
        /// </summary>
        private void DrawPausedOverlay()
        {
            _spriteBatch.Begin();
            
            // Draw semi-transparent black overlay
            Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.Black });
            _spriteBatch.Draw(pixel, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.Black * 0.5f);
            
            // Draw "PAUSED" text in center of screen
            string pausedText = "PAUSED";
            Vector2 textSize = _gameFont.MeasureString(pausedText);
            Vector2 textPosition = new Vector2(
                (SCREEN_WIDTH - textSize.X) / 2,
                (SCREEN_HEIGHT - textSize.Y) / 2
            );
            _spriteBatch.DrawString(_gameFont, pausedText, textPosition, Color.White);
            
            _spriteBatch.End();
        }

        /// <summary>
        /// Draw the game over screen
        /// </summary>
        private void DrawGameOver()
        {
            _spriteBatch.Begin();
            // TODO: Draw game over screen
            _spriteBatch.End();
        }
    }
}
