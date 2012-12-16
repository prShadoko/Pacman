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
    public class GameLoop : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Map _map;
        private Pacman _pacman;
        private Ghost[] _ghosts;

        private int _level;
		private int _live;

		private int _pause;

        private int _counter;

		private int _outgoingCounter;

        public GameLoop()
        {
            Content.RootDirectory = "Content";

            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 448;
            _graphics.PreferredBackBufferHeight = 576;
            _graphics.ApplyChanges();

			_ghosts = new Ghost[4];

			_map = new Map(_ghosts);
			_pacman = new Pacman(_map);

			Blinky blinky = new Blinky(_map, _pacman);
			Pinky pinky = new Pinky(_map, _pacman);
			Inky inky = new Inky(_map, _pacman, blinky);
			Clyde clyde = new Clyde(_map, _pacman);

			_ghosts[0] = blinky;
			_ghosts[1] = pinky;
			_ghosts[2] = inky;
			_ghosts[3] = clyde;


			_live = 3;
			_level = 1;
			this.IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
          //  _pacman.Position = _map.MapToWin(new Vector2(27, 14));

			_map.Initialize();

			_pacman.Initialize();

			_ghosts[0].Initialize();
			_ghosts[1].Initialize();
			_ghosts[2].Initialize();
			_ghosts[3].Initialize();

			_ghosts[0].Position = _map.MapToWin(new Vector2(14, 11)) - new Vector2(_map.TileSize.X / 2, 0);
			_ghosts[1].Position = _map.MapToWin(new Vector2(14, 14)) - new Vector2(_map.TileSize.X / 2, 0);
			_ghosts[2].Position = _map.MapToWin(new Vector2(12, 14)) - new Vector2(_map.TileSize.X / 2, 0);
			_ghosts[3].Position = _map.MapToWin(new Vector2(16, 14)) - new Vector2(_map.TileSize.X / 2, 0);

			_ghosts[0].Direction = Direction.LEFT;
			_ghosts[1].Direction = Direction.DOWN;
			_ghosts[2].Direction = Direction.UP;
			_ghosts[3].Direction = Direction.UP;

			_ghosts[0].level = _level;
			_ghosts[1].level = _level;
			_ghosts[2].level = _level;
			_ghosts[3].level = _level;


            _counter = 0;

			_outgoingCounter = 0;

			_pause = 0;

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
			/*if (keyboard.IsKeyDown(Keys.F))
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
			}*/
			Console.Clear();
			Console.WriteLine("Blinky\t: " + _ghosts[0].Mode);
			Console.WriteLine("Pinky\t: " + _ghosts[1].Mode);
			Console.WriteLine("Inky\t: " + _ghosts[2].Mode);
			Console.WriteLine("Clyde\t: " + _ghosts[3].Mode);
			Console.WriteLine("");
			Console.WriteLine("vie\t: " + _live);
			Console.WriteLine("Niveau\t: " + _level);

			if (_pause == 0) 
			{
				_pacman.Update(_counter);
			}

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
				if (_pause == 0 || g.Mode == GhostMode.INCOMING)
				{
					g.Update(_counter);
					if (g.Mode == GhostMode.INCOMING)
					{
						g.Update(_counter);
					}
				}
			}


			if (_map.isEmpty())
			{
				win();
			}

			int ghostIndex;
			GhostMode mode;
			if (clash(out ghostIndex, out mode))
			{
				if (mode == GhostMode.FRIGHTENED)
				{
					_pause = 60;
					_ghosts[ghostIndex].Mode = GhostMode.INCOMING;
				}
				else if (mode != GhostMode.INCOMING)
				{
					--_live;
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
			}


			if (_pause > 0)
			{
				--_pause;
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
		protected bool clash(out int ghostIndex, out GhostMode mode)
		{
			bool isClashed = false;
			ghostIndex = 0;
			mode = GhostMode.SCATTER;

			for(int i = 0; i<_ghosts.Length; ++i)
			{
				if (_map.WinToMap(_pacman.Position) == _map.WinToMap(_ghosts[i].Position))
				{
					isClashed = true;
					ghostIndex = i;
					mode = _ghosts[i].Mode;
				}
			}

			return isClashed;
		}

		protected void gameOver()
		{
			Exit();
		}

		protected void win()
		{
			//Console.WriteLine("win");
			_map = new Map(_ghosts);
			_pacman.Map = _map;
			foreach (Ghost g in _ghosts)
			{
				g.Map = _map;
			}
			++_level;
			Initialize();
			
		}
    }
}
