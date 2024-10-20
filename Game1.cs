using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Linq;

namespace blobgame55mat
{
    public class Game1 : Game
    {
        public class Coin
{
    public Vector2 Position { get; set; }
    public bool IsCollected { get; set; }

    public Coin(Vector2 position)
    {
        Position = position;
        IsCollected = false;
    }
}
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _blockTexture;
        private Texture2D _playerTexture;
        private Texture2D _spikesTexture;
        private Texture2D _block2Texture;
        private Texture2D _coinTexture;

        private const int TileSize = 32; // Size of the tiles
        private int MapWidth;
        private const int CameraWidth = 700; // Set the camera width to match the window width
        private const int CameraHeight = 600; // Set the camera height to match the window height

        private Vector2 _playerPosition;
        private Vector2 _playerVelocity;
        private bool _isJumping = false;
        private Song _backgroundMusic; // For background music
        private List<Coin> _coins;

        private const int PlayerSpeed = 3;
        private const float JumpForce = -10f;
        private const float Gravity = 0.5f;
        private int _coinsCollected = 0; // Count of collected coins
        private const int TotalCoins = 3; // Total number of coins in the game


        // Extended map with holes (0 represents void), now wider
        private static readonly int[,] Tiles = {
            { 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, // Ground
            { 2, 0, 0, 1, 0, 0, 0, 4, 0, 0, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 0, 0, 4, 0, 0, 0}, // Jumping platforms
            { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // Empty space
            { 2, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 3, 3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // Ground with voids
            { 2, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // Empty space
            { 2, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // Empty space
            { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 0, 0, 1, 0, 0, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 4, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        };

        private Vector2 _cameraPosition; // Camera position

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 700; // Set window width
            _graphics.PreferredBackBufferHeight = 600; // Set window height
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Start the player at the bottom left corner
            _playerPosition = new Vector2(50, TileSize * (Tiles.GetLength(0) - 2) - TileSize + 200); // Adjust to new tile size
            _playerVelocity = Vector2.Zero; // Initial velocity
            _cameraPosition = Vector2.Zero; // Initialize camera position
            base.Initialize();

             MapWidth = TileSize * Tiles.GetLength(1);
        }

        protected override void LoadContent()
{
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    _blockTexture = Content.Load<Texture2D>("wooden_block");
    _playerTexture = Content.Load<Texture2D>("panak");
    _spikesTexture = Content.Load<Texture2D>("spicy_spikes");
    _block2Texture = Content.Load<Texture2D>("wallblock");
    _coinTexture = Content.Load<Texture2D>("coinik");

    _backgroundMusic = Content.Load<Song>("song");
    
    // Initialize the coins list
    _coins = new List<Coin>();

    // Populate the coin positions based on the Tiles array
    for (int y = 0; y < Tiles.GetLength(0); y++)
    {
        for (int x = 0; x < Tiles.GetLength(1); x++)
        {
            if (Tiles[y, x] == 4) // Coin
            {
                Vector2 coinPosition = new Vector2(x * TileSize, _graphics.PreferredBackBufferHeight - (y + 1) * TileSize);
                _coins.Add(new Coin(coinPosition)); // Add the coin to the list
            }
        }
    }

    // Play background music if loaded successfully
    if (_backgroundMusic != null)
    {
        MediaPlayer.Play(_backgroundMusic); 
        MediaPlayer.IsRepeating = true; 
    }
}
        protected override void Update(GameTime gameTime)
{
    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
        Keyboard.GetState().IsKeyDown(Keys.Escape))
    {
        Exit();
    }

    // Player movement logic (horizontal)
    if (Keyboard.GetState().IsKeyDown(Keys.Right))
    {
        _playerVelocity.X = PlayerSpeed; // Move right
    }
    else if (Keyboard.GetState().IsKeyDown(Keys.Left))
    {
        _playerVelocity.X = -PlayerSpeed; // Move left
    }
    else
    {
        _playerVelocity.X = 0; // Stop horizontal movement if no key is pressed
    }

    // Jumping logic
    if (Keyboard.GetState().IsKeyDown(Keys.Space) && !_isJumping)
    {
        _playerVelocity.Y = JumpForce; // Apply jump force
        _isJumping = true; // Set jump state
    }

    // Apply gravity
    _playerVelocity.Y += Gravity; // Apply gravity

    // Create player rectangle for collision detection
    Rectangle playerRectangle = new Rectangle(
        (int)_playerPosition.X,
        (int)_playerPosition.Y,
        (int)(_blockTexture.Width * 2), // Scale the width
        (int)(_blockTexture.Height * 2) // Scale the height
    );

    Vector2 newPlayerPosition = _playerPosition + new Vector2(_playerVelocity.X, 0);
    Rectangle newPlayerRectangle = new Rectangle(
        (int)newPlayerPosition.X,
        (int)_playerPosition.Y,
        (int)(_blockTexture.Width * 2), // Scale the width
        (int)(_blockTexture.Height * 2) // Scale the height
    );

    for (int y = 0; y < Tiles.GetLength(0); y++)
    {
        for (int x = 0; x < Tiles.GetLength(1); x++)
        {
            if (Tiles[y, x] == 1 || Tiles[y, x] == 2) // Check for both wooden blocks and wall blocks
            {
                Rectangle blockRectangle = new Rectangle(
                    x * TileSize,
                    _graphics.PreferredBackBufferHeight - (y + 1) * TileSize,
                    (int)(_blockTexture.Width * 2), // Scale the width
                    (int)(_blockTexture.Height * 2) // Scale the height
                );

                // Handle horizontal collision
                if (newPlayerRectangle.Intersects(blockRectangle))
                {
                    if (_playerVelocity.X > 0) // Moving right
                    {
                        _playerPosition.X = blockRectangle.Left - playerRectangle.Width; // Block right movement
                    }
                    else if (_playerVelocity.X < 0) // Moving left
                    {
                        _playerPosition.X = blockRectangle.Right; // Block left movement
                    }

                    _playerVelocity.X = 0; // Stop horizontal velocity
                }
            }
        }
    }

    // Now handle vertical collisions separately
    newPlayerPosition = _playerPosition + new Vector2(0, _playerVelocity.Y);
    newPlayerRectangle = new Rectangle(
        (int)_playerPosition.X,
        (int)newPlayerPosition.Y,
        (int)(_blockTexture.Width * 2),
        (int)(_blockTexture.Height * 2)
    );

    for (int y = 0; y < Tiles.GetLength(0); y++)
    {
        for (int x = 0; x < Tiles.GetLength(1); x++)
        {
            Rectangle blockRectangle = new Rectangle(
                x * TileSize,
                _graphics.PreferredBackBufferHeight - (y + 1) * TileSize,
                (int)(_blockTexture.Width * 2),
                (int)(_blockTexture.Height * 2)
            );

            // Handle collision with blocks
            if (Tiles[y, x] == 1 || Tiles[y, x] == 2) // Wooden block or wall block
            {
                if (newPlayerRectangle.Intersects(blockRectangle))
                {
                    if (_playerVelocity.Y > 0) // Falling
                    {
                        _playerPosition.Y = blockRectangle.Top - playerRectangle.Height; // Place player on top
                        _playerVelocity.Y = 0; // Reset vertical velocity
                        _isJumping = false; // Reset jump state
                    }
                    else if (_playerVelocity.Y < 0) // Jumping up
                    {
                        _playerPosition.Y = blockRectangle.Bottom; // Adjust position if jumping up
                        _playerVelocity.Y = 0; // Reset vertical velocity
                    }
                }
            }
            // Handle collision with spikes
            if (Tiles[y, x] == 3) // Spikes
            {
                if (newPlayerRectangle.Intersects(blockRectangle))
                {
                    RestartGame(); // Kill player and respawn
                }
            }

            // Handle collision with coins
                        if (Tiles[y, x] == 4) // Coin
    {
        if (newPlayerRectangle.Intersects(blockRectangle))
        {
            // Check if the coin is already collected
            Coin coin = _coins.FirstOrDefault(c => c.Position == blockRectangle.Location.ToVector2());

            if (coin != null && !coin.IsCollected) // Coin found and not collected yet
            {
                coin.IsCollected = true; // Mark the coin as collected
                Tiles[y, x] = 0; // Remove the coin from the map
                _coinsCollected++; // Increment collected coins
                if (_coinsCollected >= TotalCoins)
                {
                    // Show winning message (or just close the game)
                    Exit(); // End the game or display winning message
                }
            }
        }
    }
}
    }
    _playerPosition += _playerVelocity;

    // Adjust camera position based on the player's position
    _cameraPosition.X = _playerPosition.X - CameraWidth / 2 + TileSize / 2; // Center horizontally
    _cameraPosition.Y = _playerPosition.Y - CameraHeight / 2 + TileSize / 2; // Center vertically

    // Clamp camera position to not exceed map bounds
    if (_cameraPosition.X < 0) _cameraPosition.X = 0;
    if (_cameraPosition.X > MapWidth - CameraWidth) _cameraPosition.X = MapWidth - CameraWidth;
    if (_cameraPosition.Y < 0) _cameraPosition.Y = 0;
    if (_cameraPosition.Y > (_graphics.PreferredBackBufferHeight - CameraHeight)) _cameraPosition.Y = (_graphics.PreferredBackBufferHeight - CameraHeight);

    // Check for void (player falling)
    if (_playerPosition.Y > _graphics.PreferredBackBufferHeight)
    {
        RestartGame(); // Reset the game
    }

    base.Update(gameTime);
}
        // Define the RestartGame method
        private void RestartGame()
{
    // Reset the player's position to a defined starting point (e.g., first platform)
    _playerPosition = new Vector2(50, TileSize * (Tiles.GetLength(0) - 2) - TileSize + 200); // Spawn above the block

    // Reset the player's velocity to zero
    _playerVelocity = Vector2.Zero; // Reset velocity to ensure no unintended movement

    // Reset the jump state
    _isJumping = false; // Allow jumping again

    // Optionally, reset the camera position here
    _cameraPosition = Vector2.Zero; // Reset the camera position

    // Reset collected coins count
    _coinsCollected = 0;

    // Reset coin collection status
    foreach (var coin in _coins)
    {
        coin.IsCollected = false; // Reset coin collection status
    }

    // Remove any coins collected and reset the map
    for (int y = 0; y < Tiles.GetLength(0); y++)
    {
        for (int x = 0; x < Tiles.GetLength(1); x++)
        {
            if (Tiles[y, x] == 4) // Coin
            {
                // Reset the tile back to its original state (coin)
                Tiles[y, x] = 4; // This puts the coin back in the tile layout
            }
        }
    }
}
        // Add the Draw method here
                        protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(Color.CornflowerBlue);
    _spriteBatch.Begin();

    // Loop through each tile and draw it
    for (int y = 0; y < Tiles.GetLength(0); y++)
    {
        for (int x = 0; x < Tiles.GetLength(1); x++)
        {
            Vector2 blockPosition = new Vector2(
                x * TileSize - _cameraPosition.X,
                _graphics.PreferredBackBufferHeight - (y + 1) * TileSize - _cameraPosition.Y
            );

            // Draw blocks
            if (Tiles[y, x] == 1)
            {
                _spriteBatch.Draw(_blockTexture, blockPosition, null, Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0);
            }
            else if (Tiles[y, x] == 2)
            {
                _spriteBatch.Draw(_block2Texture, blockPosition, null, Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0);
            }

            // Draw spikes
            if (Tiles[y, x] == 3)
            {
                _spriteBatch.Draw(_spikesTexture, blockPosition, null, Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0);
            }
        }
    }

    // Draw the coins
    foreach (var coin in _coins)
    {
        if (!coin.IsCollected) // Only draw if not collected
        {
            _spriteBatch.Draw(_coinTexture, coin.Position, Color.White);
        }
    }

    // Draw the player
    _spriteBatch.Draw(_playerTexture, new Vector2(_playerPosition.X - _cameraPosition.X, _playerPosition.Y - _cameraPosition.Y),
                      null, Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0);

    _spriteBatch.End();
    base.Draw(gameTime);
}
    }
}