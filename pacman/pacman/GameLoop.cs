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
using System.IO;

namespace pacman
{
	/// <summary>
	/// Enum the differents foods and these points.
	/// </summary>
	public enum Food
	{
		NONE = 0,
		GUM = 10,
		PACGUM = 50,
		CHERRY = 100,
		STRAWBERRY = 300,
		PEACH = 500,
		APPLE = 700,
		POMEGRANATE = 1000,
		GALAXIAN = 2000,
		BELL = 3000,
		KEY = 5000
	};

	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class GameLoop : Microsoft.Xna.Framework.Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private Texture2D[] _lifeTexture;
		private Map _map;
		private Pacman _pacman;
		private Ghost[] _ghosts;
		private int _score;
		private int _highScore;
		private int _level;
		private int _life;
		private int _pause;
		private int _ready;
		private int _counter;
		private int _outgoingCounter;
		private int _fruitGrown;
		private int _eatenGhosts;
		private SpriteFont _font;
		private int _ghostPoint;
		private bool _dead;
		private int _textureIndex;

		private bool _isOnHomeScreen;
		private HomeScreen _homeScreen;

		private SoundEffectInstance _soundOpening;
		private SoundEffectInstance _soundEatingGum;
		private SoundEffectInstance _soundEatingPacGum;
		private SoundEffectInstance _soundEatingGhost;
		private SoundEffectInstance _soundEatingFruit;
		private SoundEffectInstance _soundExtraLife;
		private SoundEffectInstance _soundDeath;

		private bool _keySPressed;
		private bool _keyEscapePressed;

		/// <summary>
		/// Constructor of the game main loop.
		/// </summary>
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

			_score = 0;
			_ghostPoint = 0;
			_life = 3;
			_level = 1;
			_ready = 60 * 4;
			this.IsMouseVisible = true;

			_isOnHomeScreen = true;
			_homeScreen = new HomeScreen(_score, _highScore);
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_keySPressed = false;
			_keyEscapePressed = false;

			_highScore = HighScore;

			_homeScreen.Initialize();
			_homeScreen.HighScore = _highScore;
			_textureIndex = 0;

			_map.Initialize();
			_pacman.Position = _map.MapToWin(new Vector2(14, 23)) - new Vector2(_map.TileSize.X / 2, 0);

			_pacman.Level = _level;
			_ghosts[0].Level = _level;
			_ghosts[1].Level = _level;
			_ghosts[2].Level = _level;
			_ghosts[3].Level = _level;

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

			_ghosts[0].ElroySpeed = 0;

			_counter = 0;

			_outgoingCounter = 0;

			_pause = 0;

			_fruitGrown = 0;

			_eatenGhosts = 0;

			_dead = false;

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			_homeScreen.LoadContent(Content);

			_lifeTexture = new Texture2D[2];
			_lifeTexture[0] = Content.Load<Texture2D>("actorsTexture");
			_lifeTexture[1] = Content.Load<Texture2D>("actorsTextureModern");

			// loading textures
			_map.LoadContent(Content);
			_pacman.LoadContent(Content);
			foreach (Ghost g in _ghosts)
			{
				g.LoadContent(Content);
			}
			_font = Content.Load<SpriteFont>("ArcadeClassic");

			SoundEffect sound = Content.Load<SoundEffect>("Tututudulu");
			_soundOpening = sound.CreateInstance();

			sound = Content.Load<SoundEffect>("Gloup");
			_soundEatingGhost = sound.CreateInstance();

			sound = Content.Load<SoundEffect>("HellYeah");
			_soundExtraLife = sound.CreateInstance();

			sound = Content.Load<SoundEffect>("MiamLulu");
			_soundEatingGum = sound.CreateInstance();

			sound = Content.Load<SoundEffect>("OuinOuinBipBip");
			_soundDeath = sound.CreateInstance();

			sound = Content.Load<SoundEffect>("Haha");
			_soundEatingPacGum = sound.CreateInstance();

			sound = Content.Load<SoundEffect>("Hmm");
			_soundEatingFruit = sound.CreateInstance();
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
			if (_isOnHomeScreen)
			{
				if (!keyboard.IsKeyDown(Keys.Escape) && _keyEscapePressed)
				{
					_keyEscapePressed = false;
				}

				if (keyboard.IsKeyDown(Keys.Enter) || keyboard.IsKeyDown(Keys.Space))
				{
					_isOnHomeScreen = false;
					_soundOpening.Play();
				}
				else if (keyboard.IsKeyDown(Keys.Escape) && ! _keyEscapePressed)
				{
					this.Exit();
				}
				else if (keyboard.IsKeyDown(Keys.S) && !_keySPressed)
				{
					//TODO: Change index
					_keySPressed = true;
					_homeScreen.TextureIndex += 1;
					_pacman.TextureIndex += 1;
					this.TextureIndex += 1;
					foreach (Ghost g in _ghosts)
					{
						g.TextureIndex++;
					}
				}
				else if ( ! keyboard.IsKeyDown(Keys.S) )
				{
					_keySPressed = false;
				}

				_homeScreen.Update(_counter);
			}
			else
			{
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

				if (keyboard.IsKeyDown(Keys.Escape) && ! _keyEscapePressed)
				{
					GameOver();
					_keyEscapePressed = true;
				}
				if (!keyboard.IsKeyDown(Keys.Escape) && _keyEscapePressed)
				{
					_keyEscapePressed = false;
				}

				int prevScore = _score;
				_pacman.UpdateDirection();

				if (_pause == 1 && _dead)
				{
					_dead = false;
					if (_life <= 0)
					{
						GameOver();
					}
					else
					{
						Initialize();
						_ready = 60 * 2;
					}
				}

				if (_pause == 0 && _ready == 0)
				{
					_pacman.Update(_counter);

					_score += (int)_pacman.Eaten;
					if (_pacman.Eaten == Food.GUM)
					{
						_soundEatingGum.Play();
					}
					else if (_pacman.Eaten == Food.PACGUM)
					{
						_soundEatingPacGum.Play();

						if (_level <= 17 || _level == 19)
						{
							foreach (Ghost g in _ghosts)
							{
								if (g.Mode != GhostMode.INCOMING)
								{
									g.Mode = GhostMode.FRIGHTENED;
								}
							}
							_pacman.Frightening = true;
							_eatenGhosts = 0;
						}
					}
					else if (_pacman.Eaten != Food.NONE) // If it is a fruit
					{
						_soundEatingFruit.Play();
						_fruitGrown = 0;
					}
					_pacman.Eaten = Food.NONE;

					if (_map.NbGum == 70 || _map.NbGum == 170)
					{
						_map.GrowFruit(_level);
						_fruitGrown = 10 * 60;
					}

					if (_outgoingCounter < _ghosts.Length - 1)
					{
						if (_ghosts[_outgoingCounter].Mode != GhostMode.OUTGOING && (// Attention, ceci est du code très sale !!
							_outgoingCounter == 0 ||												// pour que Blinky et Pinky ne reste pas bloqué
							_level == 1 && _outgoingCounter == 1 && _map.NbGum <= (244 - 30) ||		// Fait sortir Inky après 30 dots au niveau 1
							_level == 1 && _outgoingCounter == 2 && _map.NbGum <= 244 - 30 - 60 ||	// Fait sortir Clyde après Inky après 60 dots au niveau 1
							_level == 2 && _outgoingCounter == 1 ||									// Fait sortir Inky après 0 dots au niveau 2
							_level == 2 && _outgoingCounter == 2 && _map.NbGum <= 244 - 50 ||		// Fait sortir Clyde après 50 dots au niveau 2
							_level > 2))															// fait sortir tout le monde au dela du niveau 2
						{
							++_outgoingCounter;
							_ghosts[_outgoingCounter].Mode = GhostMode.OUTGOING;
						}
					}

					foreach (Ghost g in _ghosts)
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

					if (_map.IsEmpty())
					{
						Win();
					}

					int ghostIndex;
					GhostMode mode;
					if (Clash(out ghostIndex, out mode))
					{
						if (mode == GhostMode.FRIGHTENED)
						{
							_soundEatingGhost.Play();
							++_eatenGhosts;
							_pause = 30;
							_ghosts[ghostIndex].Drawable = false;
							_ghosts[ghostIndex].Mode = GhostMode.INCOMING;
							_ghostPoint = (int)Math.Pow(2, _eatenGhosts) * 100;
							_score += _ghostPoint;
							if (_eatenGhosts == 4)
							{
								_pacman.Frightening = false;
							}
						}
						else if (mode != GhostMode.INCOMING)
						{
							--_life;
							_dead = true;
							_pause = 2 * 60;
							foreach (Ghost g in _ghosts)
							{
								g.Drawable = false;
							}
							_soundDeath.Play();
						}
					}

					if (--_fruitGrown == 0)
					{
						_map.DecayFruit();
					}
				}

				if (_pause > 0)
				{
					if (--_pause == 0)
					{
						foreach (Ghost g in _ghosts)
						{
							g.Drawable = true;
						}
					}
				}

				if (_ready > 0)
				{
					--_ready;
				}

				if (prevScore / 10000 != _score / 10000 && _life < 5)
				{
					++_life;
					_soundExtraLife.Play();
				}
			}
			++_counter;
			_counter %= 60;

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			Vector2 textPos = new Vector2(0, 0);

			GraphicsDevice.Clear(Color.Black);
			_spriteBatch.Begin();

			if (_isOnHomeScreen)
			{
				_homeScreen.Draw(_spriteBatch);
			}
			else
			{
				_map.Draw(_spriteBatch);
				if (_ready < 60 * 2)
				{
					if (_pause != 0 && !_dead)
					{
						// --- Affichage du score quand on mange un fantome --- //
						textPos = _pacman.Position;
						textPos.X -= _map.TileSize.X;
						textPos.Y -= _map.TileSize.Y / 2;
						_spriteBatch.DrawString(_font, _ghostPoint.ToString(), textPos, Color.Cyan, 0f, new Vector2(0, 0), 0.6f, new SpriteEffects(), 1f);
					}
					else
					{
						if (_ready > 0)
							_pacman.DrawInit(_spriteBatch);
						else if (_dead)
							_pacman.DrawDeath(_spriteBatch);
						else
							_pacman.Draw(_spriteBatch);
					}

					foreach (Ghost g in _ghosts)
					{
						if (g.Drawable)
						{
							g.Draw(_spriteBatch);
						}
					}
				}
				else
				{
					textPos = _map.MapToWin(new Vector2(8, 10));
					textPos.X += _map.TileSize.X / 2;
					textPos.X += 3;
					_spriteBatch.DrawString(_font, "player one", textPos, Color.Cyan);
				}

				if (_ready != 0)
				{
					textPos = _map.MapToWin(new Vector2(10, 16));
					textPos.X += _map.TileSize.X / 2;
					_spriteBatch.DrawString(_font, "ready!", textPos, Color.Yellow);
				}


				// --- affichage du texte --- //
				textPos = _map.MapToWin(new Vector2(9, -3));
				textPos.Y -= _map.TileSize.Y / 2;
				_spriteBatch.DrawString(_font, "HIGH  SCORE", textPos, Color.White);

				string text = _score.ToString();
				textPos = _map.MapToWin(new Vector2(6 - text.Length, -2));
				_spriteBatch.DrawString(_font, text, textPos, Color.White);

				text = _highScore.ToString();
				if (_score > _highScore)
				{
					text = _score.ToString();
				}
				textPos = _map.MapToWin(new Vector2(16 - text.Length, -2));
				_spriteBatch.DrawString(_font, text, textPos, Color.White);

				if (_counter % 30 <= 15)
				{
					textPos = _map.MapToWin(new Vector2(2, -3));
					textPos.Y -= _map.TileSize.Y / 2;
					_spriteBatch.DrawString(_font, "1UP", textPos, Color.White);
				}

				// --- affichage des vies --- //
				Rectangle lifeClipping = new Rectangle(
								2 * (int)_ghosts[0].TileSize.X,
								11 * (int)_ghosts[0].TileSize.Y,
								(int)_ghosts[0].TileSize.X,
								(int)_ghosts[0].TileSize.Y);
				Vector2 lifePos = _map.MapToWin(new Vector2(2, 30));
				lifePos.Y += _map.TileSize.Y / 2;
				for (int i = 0; i < _life; ++i)
				{
					_spriteBatch.Draw(_lifeTexture[_textureIndex], lifePos, lifeClipping, Color.White);
					lifePos.X += _map.TileSize.X * 2;
				}
			}

			_spriteBatch.End();

			base.Draw(gameTime);
		}

		/// <summary>
		/// Test if pacman clash ghost.
		/// </summary>
		/// <returns>True if pacman clash ghost, else return false.</returns>
		protected bool Clash(out int ghostIndex, out GhostMode mode)
		{
			bool isClashed = false;
			ghostIndex = 0;
			mode = GhostMode.SCATTER;

			for (int i = 0; i < _ghosts.Length; ++i)
			{
				if (_map.WinToMap(_pacman.Position) == _map.WinToMap(_ghosts[i].Position) && _ghosts[i].Mode != GhostMode.INCOMING)
				{
					isClashed = true;
					ghostIndex = i;
					mode = _ghosts[i].Mode;
				}
			}

			return isClashed;
		}

		/// <summary>
		/// Function to call when there is a gameOver. It reinitilize the game to return to the home screen.
		/// </summary>
		protected void GameOver()
		{
			if (_score > _highScore)
			{
				HighScore = _score;
			}
			Initialize();
			_map.ResetMap();
			_isOnHomeScreen = true;
			_homeScreen.Score = _score;
			_score = 0;
			_life = 3;
			_level = 1;
			_homeScreen.HighScore = _highScore;
			_ready = 60 * 4;
		}

		/// <summary>
		/// Function to call when the pacman clean a level. It initialize the next level.
		/// </summary>
		protected void Win()
		{
			++_level;
			_map.ResetMap();
			Initialize();
		}

		/// <summary>
		/// Property to access to high score in the hard drive disk.
		/// </summary>
		public int HighScore
		{
			get
			{
				int score = 0;

				try
				{
					FileStream fs = new FileStream("highscore.pac", FileMode.Open, FileAccess.Read);
					StreamReader sr = new StreamReader(fs);
					score = int.Parse(sr.ReadLine());
					sr.Close();
					fs.Close();
				}
				catch (Exception ex)
				{
					Console.WriteLine("Erreur dans le fichier highscore.pac : " + ex.Message);
				}
				return score;
			}
			set
			{
				FileStream fs = new FileStream("highscore.pac", FileMode.OpenOrCreate, FileAccess.Write);
				StreamWriter sw = new StreamWriter(fs);
				sw.WriteLine(_score);
				sw.Close();
				fs.Close();
			}
		}

		/// <summary>
		/// Property to access to the texture index.
		/// </summary>
		public int TextureIndex
		{
			get
			{
				return _textureIndex;
			}
			set
			{
				if (value >= _lifeTexture.Length)
				{
					//Console.WriteLine("\t"+_texture.Length);
					_textureIndex = 0;
				}
				else
				{
					_textureIndex = value;
				}
			}
		}
	}
}
