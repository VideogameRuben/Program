# Beat 'Em Up Game - MonoGame

A 2D side-scrolling beat 'em up game built with MonoGame, inspired by classic arcade games like Final Fight.

## Project Structure

```
BeatEmUpGame/
├── BeatEmUpGame.csproj    # Project file with dependencies
├── Program.cs             # Application entry point
├── Game1.cs               # Main game class (game loop, state management)
├── Player.cs              # Player character (movement, physics, input)
├── Camera.cs              # 2D camera system (follows player)
├── GameState.cs           # Game state enumeration
├── Content/               # Game assets (sprites, sounds, fonts)
│   └── Content.mgcb      # MonoGame Content Pipeline project
├── app.manifest          # Windows manifest for DPI awareness
└── Icon.ico              # Application icon (optional)
```

## Features Implemented

### 1. Game Loop & State Management
- **Game1.cs**: Core game class with proper MonoGame lifecycle
  - `Initialize()`: Sets up game objects and systems
  - `LoadContent()`: Loads sprites and assets
  - `Update()`: Game logic (60 FPS)
  - `Draw()`: Rendering pipeline

### 2. Game States
- **Menu**: Title screen (Press Enter to start)
- **Playing**: Active gameplay
- **Paused**: Freeze game (Press P to toggle)
- **GameOver**: End screen (Press Enter to restart)

### 3. Player System
- **Movement**: Left/Right arrow keys or A/D
- **Jump**: Space or W key
- **Physics**: Gravity, velocity, ground collision
- **Sprite**: Placeholder rendering (ready for sprite sheets)
- **Facing Direction**: Sprite flips based on movement

### 4. Camera System
- **Horizontal Scrolling**: Follows player smoothly
- **Smooth Following**: Lerp interpolation for fluid camera movement
- **Boundary Clamping**: Keeps camera within level bounds
- **Transform Matrix**: Applied to SpriteBatch for world-to-screen rendering

### 5. Input Handling
- **Keyboard Controls**:
  - Arrow Keys / WASD: Movement
  - Space / W: Jump
  - P: Pause
  - Enter: Menu navigation
  - Escape: Exit game

## Controls

| Action | Keys |
|--------|------|
| Move Left | Left Arrow / A |
| Move Right | Right Arrow / D |
| Jump | Space / W |
| Pause | P |
| Menu Select | Enter |
| Exit | Escape |

## Building and Running

### Prerequisites
- .NET 6.0 SDK or higher
- MonoGame 3.8.1 or higher

### Build Instructions

1. **Restore Dependencies**:
   ```bash
   dotnet restore
   ```

2. **Build the Project**:
   ```bash
   dotnet build
   ```

3. **Run the Game**:
   ```bash
   dotnet run
   ```

## Adding Content (Sprite Sheets)

To add sprite sheets and other game assets:

1. Place your `.png` files in the `Content/` folder
2. Edit `Content/Content.mgcb` and add:
   ```xml
   <Compile Include="player.png">
     <Name>player</Name>
     <Importer>TextureImporter</Importer>
     <Processor>TextureProcessor</Processor>
   </Compile>
   ```
3. Load in `Game1.cs` using:
   ```csharp
   Texture2D playerSprite = Content.Load<Texture2D>("player");
   ```

## Game Architecture

### Rendering Pipeline
1. **Clear Screen**: Set background color
2. **Begin SpriteBatch**: Apply camera transformation
3. **Draw Background**: Parallax layers (future)
4. **Draw Entities**: Player, enemies, items
5. **End SpriteBatch**: Complete world rendering
6. **Draw UI**: HUD elements (no camera transform)

### Update Loop
1. **Read Input**: Keyboard/gamepad state
2. **Update State**: State-specific logic
3. **Update Physics**: Gravity, velocity, collisions
4. **Update Camera**: Follow player
5. **Update AI**: Enemy behavior (future)

## Next Steps / TODO

- [ ] Add enemy AI and combat system
- [ ] Implement attack animations and hitboxes
- [ ] Add sprite sheet animation system
- [ ] Create parallax background scrolling
- [ ] Add sound effects and music
- [ ] Implement combo system
- [ ] Add health and score system
- [ ] Create multiple levels
- [ ] Add game difficulty settings
- [ ] Implement local co-op multiplayer

## Code Documentation

All classes are fully commented with XML documentation:
- **Summaries**: Explain class/method purpose
- **Parameters**: Describe method inputs
- **Returns**: Explain return values
- **Remarks**: Additional implementation notes

## License

This is a learning project. Feel free to use and modify as needed.
