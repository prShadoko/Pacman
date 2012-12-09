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
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private Map _map;
		private Pacman _pacman;
		private Ghost[] _ghosts;

        private int _counter;

		public Game1()
		{
			Content.RootDirectory = "Content";

			_graphics = new GraphicsDeviceManager(this);
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
			_map = new Map();
			_pacman = new Pacman(_map);

            Blinky blinky = new Blinky(_map, _pacman);
            Pinky  pinky  = new Pinky (_map, _pacman);
            Inky   inky   = new Inky  (_map, _pacman, blinky);
            Clyde  clyde  = new Clyde (_map, _pacman);
            
            _ghosts = new Ghost[] {
				blinky,
				pinky,
				inky,
				clyde
			};

            _ghosts[0].Position = new Vector2(10 * 16 + 16 / 2, 11 * 16 + 16 / 2);
            _ghosts[0].Speed = 0.85f;
            _ghosts[1].Position = new Vector2(2 * 16 + 16 / 2, 1 * 16 + 16 / 2);
            _ghosts[1].Speed = 0.80f;
            _ghosts[2].Position = new Vector2(26 * 16 + 16 / 2, 26 * 16 + 16 / 2);
            _ghosts[2].Speed = 0.75f;
            _ghosts[3].Position = new Vector2(2 * 16 + 16 / 2, 28 * 16 + 16 / 2);
            _ghosts[3].Speed = 0.5f;
            _counter = 0;

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
			_pacman.LoadContent(Content);
			foreach (Ghost g in _ghosts)
			{
				g.LoadContent(Content);
			}
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			Content.Unload();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			KeyboardState keyboard = Keyboard.GetState();
            
			_pacman.Update(_counter);
			foreach (Ghost g in _ghosts)
			{
                g.Update(_counter);
			}


            ++_counter;
            if (_counter % 60 == 0) _counter = 0;

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
			_pacman.Draw(gameTime, _spriteBatch);
			foreach (Ghost g in _ghosts)
			{
				g.Draw(gameTime, _spriteBatch);
			}

			_spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
