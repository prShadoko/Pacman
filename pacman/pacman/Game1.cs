using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace pacman
{
    public enum Direction { UP, DOWN, LEFT, RIGHT };

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //private Pacman _pacman;
        private Map _map;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            // changing th back buffer size changes the window size (when in windowed mode)
            _graphics.PreferredBackBufferWidth = 448;
            _graphics.PreferredBackBufferHeight = 576;
            _graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
           // _pacman = new Pacman(_map);
            _map = new Map();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);


            // loading textures
            _map.LoadContent(Content);

            //_pacman.Texture = Content.Load<Texture2D>("pacman");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
		Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            /*KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Right))
            {
                _pacman.NextDirection = Direction.RIGHT;
            }
            else if (keyboard.IsKeyDown(Keys.Left))
            {
                _pacman.NextDirection = Direction.LEFT;
            }
            else if (keyboard.IsKeyDown(Keys.Up))
            {
                _pacman.NextDirection = Direction.UP;
            }
            else if (keyboard.IsKeyDown(Keys.Down))
            {
                _pacman.NextDirection = Direction.DOWN;
            }

            _pacman.move();*/

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            _map.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
