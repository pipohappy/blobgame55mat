using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace blobgame55mat
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _blockTexture;
        private Texture2D _playerTexture;

        private const int TileSize = 32; // Size of the tiles
        private Vector2 _playerPosition;
        private Vector2 _playerVelocity;
        private bool _isJumping = false;

        private const int PlayerSpeed = 4;
        private const float JumpForce = -10f;
        private const float Gravity = 0.5f;

        // Extended map with holes (0 represents void), now wider
        private static readonly int[,] Tiles = {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, // Ground
            { 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // Jumping platforms
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // Empty space
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // Ground with voids
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // Empty space
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, // Empty space
            { 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}  // Ground with voids
        };

        private Vector2 _cameraPosition; // Camera position

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 800; // Set window width
            _graphics.PreferredBackBufferHeight = 600; // Set window height
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Start the player at the bottom left corner
             _playerPosition = new Vector2(50, TileSize * (Tiles.GetLength(0) - 2));// Start at height of the blocks
            _playerVelocity = Vector2.Zero; // Initial velocity
            _cameraPosition = Vector2.Zero; // Initialize camera position
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _blockTexture = Content.Load<Texture2D>("wooden_block"); // Block texture
            _playerTexture = Content.Load<Texture2D>("panak"); // Player texture

            // Check if textures are loaded successfully
            if (_blockTexture == null || _playerTexture == null)
            {
                System.Diagnostics.Debug.WriteLine("Failed to load texture.");
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Move the player
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _playerPosition.X += PlayerSpeed; // Move right
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _playerPosition.X -= PlayerSpeed; // Move left
            }

            // Jumping
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !_isJumping)
            {
                _playerVelocity.Y = JumpForce; // Apply jump force
                _isJumping = true; // Set jump state
            }

            // Apply gravity
            _playerVelocity.Y += Gravity; // Apply gravity

            // Update player position
            _playerPosition += _playerVelocity;

            // Camera only follows player horizontally
            _cameraPosition.X = _playerPosition.X - _graphics.PreferredBackBufferWidth / 2 + TileSize / 2; // Center the camera horizontally
            _cameraPosition.Y = 0; // Keep the camera's vertical position constant

            // Simple collision detection with the ground and platforms
            for (int y = 0; y < Tiles.GetLength(0); y++)
            {
                for (int x = 0; x < Tiles.GetLength(1); x++)
                {
                    if (Tiles[y, x] == 1) // Only check if the tile is a block
                    {
                        // Define the block rectangle with potential borders
                        Rectangle blockRectangle = new Rectangle(
                            x * TileSize,
                            _graphics.PreferredBackBufferHeight - (y + 1) * TileSize,
                            TileSize,
                            TileSize);

                        Rectangle playerRectangle = new Rectangle(
                            (int)_playerPosition.X,
                            (int)_playerPosition.Y,
                            TileSize, // Player size matches block size
                            TileSize); // Player size matches block size

                        // Check for intersection between the player and block
                        if (playerRectangle.Intersects(blockRectangle))
                        {
                            // Adjust the player's position if colliding with blocks
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

                            // Optional: Handle horizontal collisions
                            if (_playerVelocity.X > 0) // Moving right
                            {
                                _playerPosition.X = blockRectangle.Left - playerRectangle.Width; // Adjust position if colliding from the left
                            }
                            else if (_playerVelocity.X < 0) // Moving left
                            {
                                _playerPosition.X = blockRectangle.Right; // Adjust position if colliding from the right
                            }
                        }
                    }
                }
            }


            // Check if the player has fallen into the void (assuming void is below the lowest block)
            if (_playerPosition.Y > _graphics.PreferredBackBufferHeight)
            {
                RestartGame(); // Reset the game
            }

            // Ensure the player doesn't fall below ground level
            if (_playerPosition.Y < TileSize)
            {
                _playerPosition.Y = TileSize; // Reset to ground level
                _playerVelocity.Y = 0; // Reset vertical velocity
                _isJumping = false; // Reset jump state
            }

            base.Update(gameTime);
        }

                        private void RestartGame()
        {
            // Set the player position to a defined starting point, e.g., first platform
            _playerPosition = new Vector2(50, TileSize * (Tiles.GetLength(0) - 2) - TileSize); // Spawn above the block

            // Reset the player's velocity to zero
            _playerVelocity = Vector2.Zero; // Reset velocity to ensure no unintended movement

            // Reset the jump state
            _isJumping = false; // Allow jumping again

            // Optionally, you might want to reset the camera position here
            _cameraPosition = Vector2.Zero; // Reset the camera position
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Drawing tiles based on the Tiles map
            for (int y = 0; y < Tiles.GetLength(0); y++)
            {
                for (int x = 0; x < Tiles.GetLength(1); x++)
                {
                    if (Tiles[y, x] == 1) // Only draw blocks
                    {
                        // Adjust the block's position based on the camera
                        _spriteBatch.Draw(_blockTexture, new Rectangle(
                            (x * TileSize) - (int)_cameraPosition.X,
                            _graphics.PreferredBackBufferHeight - (y + 1) * TileSize, // No change in Y position
                            TileSize,
                            TileSize), Color.White);
                    }
                }
            }

            // Draw the player
            _spriteBatch.Draw(_playerTexture, new Rectangle(
                (int)_playerPosition.X - (int)_cameraPosition.X,
                (int)_playerPosition.Y,
                TileSize, // Player size matches block size
                TileSize), // Player size matches block size
                Color.Red); // Change color to differentiate player

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
