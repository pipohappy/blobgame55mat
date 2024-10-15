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

        // Tile size and map data
        private const int TileSize = 16;
        private static readonly int[,] Tiles = {
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
            {0, 1, 0, 0, 0, 0, 1, 1, 0, 0 },
            {0, 1, 0, 1, 1, 1, 1, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            {0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
            {1, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
        };

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 800; // Set window width
            _graphics.PreferredBackBufferHeight = 600; // Set window height
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _blockTexture = Content.Load<Texture2D>("wooden_block"); // Ensure this matches your content

            // Check if texture is loaded successfully
            if (_blockTexture == null)
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < Tiles.GetLength(1); y++)
                {
                    int tileType = Tiles[x, y];
                    if (tileType == 1)
                    {
                        _spriteBatch.Draw(_blockTexture, 
                            new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize), 
                            Color.White);
                    }
                }
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
