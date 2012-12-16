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

        private int _level;
		private int _live;

        private int _counter;

		private int _outgoingCounter;

        public Game1()
        {
            Content.RootDirectory = "Content";

            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 448;
            _graphics.PreferredBackBufferHeight = 576;
            _graphics.ApplyChanges();


			_live = 3;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
			this.IsMouseVisible = true;
            _map = new Map();
            _pacman = new Pacman(_map);
          //  _pacman.Position = _map.MapToWin(new Vector2(27, 14));

            Blinky blinky = new Blinky(_map, _pacman);
            Pinky pinky = new Pinky(_map, _pacman);
            Inky inky = new Inky(_map, _pacman, blinky);
            Clyde clyde = new Clyde(_map, _pacman);

			blinky.Position = _map.MapToWin(new Vector2(14, 11)) - new Vector2(_map.TileSize.X / 2, 0);
			pinky.Position = _map.MapToWin(new Vector2(14, 14)) - new Vector2(_map.TileSize.X / 2, 0);
			inky.Position = _map.MapToWin(new Vector2(12, 14)) - new Vector2(_map.TileSize.X / 2, 0);
			clyde.Position = _map.MapToWin(new Vector2(16, 14)) - new Vector2(_map.TileSize.X / 2, 0);

			pinky.Direction = Direction.DOWN;
			inky.Direction = Direction.UP;
			clyde.Direction = Direction.UP;

            _ghosts = new Ghost[] {
				blinky,
				pinky,
				inky,
				clyde
			};

            _counter = 0;

			_outgoingCounter = 0;

            _level = 1;

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
			if (keyboard.IsKeyDown(Keys.F))
			{
				_ghosts[0].Mode = GhostMode.FRIGHTENED;
				_ghosts[1].Mode = GhostMode.FRIGHTENED;
				_ghosts[2].Mode = GhostMode.FRIGHTENED;
				_ghosts[3].Mode = GhostMode.FRIGHTENED;
			}
			else if (keyboard.IsKeyDown(Keys.S))
			{
				_ghosts[0].Mode = GhostMode.SCATTER;
				_ghosts[1].Mode = GhostMode.SCATTER;
				_ghosts[2].Mode = GhostMode.SCATTER;
				_ghosts[3].Mode = GhostMode.SCATTER;
			}
			else if (keyboard.IsKeyDown(Keys.C))
			{
				_ghosts[0].Mode = GhostMode.CHASE;
				_ghosts[1].Mode = GhostMode.CHASE;
				_ghosts[2].Mode = GhostMode.CHASE;
				_ghosts[3].Mode = GhostMode.CHASE;
			}
			else if (keyboard.IsKeyDown(Keys.I))
			{
				_ghosts[0].Mode = GhostMode.INCOMING;
				_ghosts[1].Mode = GhostMode.INCOMING;
				_ghosts[2].Mode = GhostMode.INCOMING;
				_ghosts[3].Mode = GhostMode.INCOMING;
			}
			/*Console.WriteLine(_ghosts[0].Mode);
			Console.WriteLine(_ghosts[1].Mode);
			Console.WriteLine(_ghosts[2].Mode);
			Console.WriteLine(_ghosts[3].Mode);
			Console.WriteLine("");*/
			

			_pacman.Update(_counter);

			if (_outgoingCounter < _ghosts.Length - 1)
			{
				if( _ghosts[_outgoingCounter].Mode != GhostMode.OUTGOING )
				{
					++_outgoingCounter;
					_ghosts[_outgoingCounter].Mode = GhostMode.OUTGOING;
				}
			}
			foreach (Ghost g in _ghosts)
			{
                g.Update(_counter);
			}

			if (clash())
			{

				--_live;

				Console.Write(_live);
				if (_live <= 0)
				{
					//TODO: Game Over
					gameOver();
				}
				else
				{
					Initialize();
				}
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

            _map.Draw(_spriteBatch);
            _pacman.Draw(_spriteBatch);
            foreach (Ghost g in _ghosts)
            {
                g.Draw(_spriteBatch);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

		/// <summary>
		/// Test if pacman clash ghost.
		/// </summary>
		/// <returns>True if pacman clash ghost, else return false.</returns>
		protected bool clash()
		{
			bool isClashed = false;

			foreach (Ghost g in _ghosts)
			{
				if ( _map.WinToMap(_pacman.Position) == _map.WinToMap(g.Position) )
				{
					isClashed = true;
				}
			}

			return isClashed;
		}

		protected void gameOver()
		{
			Exit();
		}
    }
}
