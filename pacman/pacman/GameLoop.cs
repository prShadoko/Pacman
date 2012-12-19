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
	public enum Food {
		NONE = 0,
		GUM = 1,
		PACGUM = 2,
		CHERRY = 3,
		STRAWBERRY = 4,
		PEACH = 5,
		APPLE = 6, 
		POMEGRANATE = 7,
		GALAXIAN = 8,
		BELL = 9,
		KEY = 10
	};

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
		private int _score;
		private int _level;
		private int _life;
		private int _pause;
		private int _counter;
		private int _outgoingCounter;
		private /*const*/ int[] _foodValue = { 0, 10, 50, 200, 100, 300, 500, 700, 1000, 2000, 3000, 5000 };
		private SpriteFont _scoreFont;
		private int _eatenGhosts;


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


			_life = 3;
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

			_map.Initialize();

			_pacman.Initialize();

			_pacman.Position = _map.MapToWin(new Vector2(14, 23));

			_ghosts[0].Level = _level;
			_ghosts[1].Level = _level;
			_ghosts[2].Level = _level;
			_ghosts[3].Level = _level;

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

			_ghosts[0].ElroySpeed = 0f;

			_counter = 0;

			_outgoingCounter = 0;

			_pause = 0;

			_score = 0;

			_eatenGhosts = 0;

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
			_scoreFont = Content.Load<SpriteFont>("ScoreFont");
			//_lifeTexture = content.Load<Texture2D>("actorsTexture");
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

			/*
			Console.Clear();
			Console.WriteLine("Blinky\t: " + _ghosts[0].Mode);
			Console.WriteLine("Pinky\t: " + _ghosts[1].Mode);
			Console.WriteLine("Inky\t: " + _ghosts[2].Mode);
			Console.WriteLine("Clyde\t: " + _ghosts[3].Mode);
			Console.WriteLine("");
			Console.WriteLine("Score\t: " + _score);
			Console.WriteLine("vie\t: " + _life);
			Console.WriteLine("Niveau\t: " + _level);
			Console.WriteLine("Gum\t: " + _map.NbGum);
			Console.WriteLine("");
			Console.WriteLine("Speed Pacman\t: " + _pacman.Speed);
			Console.WriteLine("Speed Blinky\t: " + _ghosts[0].Speed);
			Console.WriteLine("Speed Pinky\t: " + _ghosts[1].Speed);
			Console.WriteLine("Speed Inky\t: " + _ghosts[2].Speed);
			Console.WriteLine("Speed Clyde\t: " + _ghosts[3].Speed);
			//*/

			if (_pause == 0)
			{
				_pacman.Update(_counter);
				UpdateScore();
				if (_pacman.Eaten == Food.PACGUM)
				{
					if (_level <= 17 || _level == 19)
					{
						SetGhostsMode(GhostMode.FRIGHTENED);
						_eatenGhosts = 0;
					}
				}
				_pacman.Eaten = Food.NONE;
			}

			if (_outgoingCounter < _ghosts.Length - 1)
			{
				if( _ghosts[_outgoingCounter].Mode != GhostMode.OUTGOING && (// Attention, ceci est du code tr�s sale !!
					_outgoingCounter == 0 ||												// pour que Blinky et Pinky ne reste pas bloqu�
					_level == 1 && _outgoingCounter == 1 && _map.NbGum <= (244 - 30) ||		// Fait sortir Inky apr�s 30 dots au niveau 1
					_level == 1 && _outgoingCounter == 2 && _map.NbGum <= 244 - 30 - 60 ||	// Fait sortir Clyde apr�s Inky apr�s 60 dots au niveau 1
					_level == 2 && _outgoingCounter == 1 ||									// Fait sortir Inky apr�s 0 dots au niveau 2
					_level == 2 && _outgoingCounter == 2 && _map.NbGum <= 244 - 50 ||		// Fait sortir Clyde apr�s 50 dots au niveau 2
					_level > 2))															// fait sortir tout le monde au dela du niveau 2
				{
					++_outgoingCounter;
					_ghosts[_outgoingCounter].Mode = GhostMode.OUTGOING;
				}
			}
			foreach (Ghost g in _ghosts)
			{
				if (_pause == 0)
				{
					g.Update(_counter);
					if (g.Mode == GhostMode.INCOMING)
					{
						g.Update(_counter);
					}
					else if (g.NbMovement > 1)
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
					++_eatenGhosts;
					_pause = 30;
					_ghosts[ghostIndex].Mode = GhostMode.INCOMING;
					_score += (int)Math.Pow(2, _eatenGhosts) * 100;
				}
				else if (mode != GhostMode.INCOMING)
				{
					--_life;
					if (_life <= 0)
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

			/*
			// --- affichage du texte --- //
			Vector2 textPos = MapToWin(new Vector2(9, -3));
			//scorePos.Y -= _spriteSize.Y / 2;
			spriteBatch.DrawString(_scoreFont, "HIGH SCORE", textPos, Color.White);

			string text = _score.ToString();
			textPos = MapToWin(new Vector2(7 - text.Length, -2));
			spriteBatch.DrawString(_scoreFont, _score.ToString(), textPos, Color.White);

			if (_1up && _drawCounter * 2 < _blinkInterval )
			{
				textPos = MapToWin(new Vector2(3, -3));
				spriteBatch.DrawString(_scoreFont, "1UP", textPos, Color.White);
			}

			// --- affichage des vies --- //
			Rectangle lifeClipping = new Rectangle(
							2 * (int)_ghosts[0].TileSize.X,
							11 * (int)_ghosts[0].TileSize.Y,
							(int)_ghosts[0].TileSize.X,
							(int)_ghosts[0].TileSize.Y);
			Vector2 lifePos = MapToWin(new Vector2(2,30));
			lifePos.Y += TileSize.Y / 2;
			for(int i = 0; i < _life; ++i)
			{
				spriteBatch.Draw(_lifeTexture, lifePos, lifeClipping, Color.White);
				lifePos.X += TileSize.X * 2;
			}
			*/

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

			for (int i = 0; i < _ghosts.Length; ++i)
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
			/*_map = new Map(_ghosts);
			_pacman.Map = _map;
			foreach (Ghost g in _ghosts)
			{
				g.Map = _map;
			}*/
			++_level;
			_map.ResetMap();
			Initialize();
		}

		protected void UpdateScore()
		{
			int prevScore = _score;
			_score += _foodValue[(int)_pacman.Eaten];
			if (prevScore / 10000 != _score / 10000)
			{
				++_life;
			}
		}
	
		protected void SetGhostsMode(GhostMode mode)
		{
			foreach (Ghost g in _ghosts)
			{
				g.Mode = mode;
			}
		}
	}
}
