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
	public enum Food {
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
		private Texture2D _lifeTexture;
		private Map _map;
		private Pacman _pacman;
		private Ghost[] _ghosts;
		private int _score;
		private int _highScore;
		private int _level;
		private int _life;
		private int _pause;
		private int _counter;
		private int _outgoingCounter;
		private int _eatenGhosts;
		private SpriteFont _font;
		private int _ghostPoint;
		private int _ready;

        private bool _isOnHomeScreen;
        private HomeScreen _homeScreen;

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
			_highScore = HighScore;
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

            _homeScreen.Initialize();

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

			_eatenGhosts = 0;

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
            _homeScreen.LoadContent(Content);

			_lifeTexture = Content.Load<Texture2D>("actorsTexture");

			// loading textures
			_map.LoadContent(Content);
			_pacman.LoadContent(Content);
			foreach (Ghost g in _ghosts)
			{
				g.LoadContent(Content);
			}
			_font = Content.Load<SpriteFont>("ArcadeClassic");
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

            if (_isOnHomeScreen)
            {
                if (keyboard.IsKeyDown(Keys.Enter) || keyboard.IsKeyDown(Keys.Space))
                {
                    _isOnHomeScreen = false;
                }
                else if (keyboard.IsKeyDown(Keys.Escape))
                {
                    this.Exit();
                }

                _homeScreen.Update(_counter);
            }
            else
            {
			    //*
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

			    int prevScore = _score;
			    _pacman.UpdateDirection();
			    if (_pause == 0 && _ready == 0)
			    {
				    _pacman.Update(_counter);

				    _score += (int)_pacman.Eaten;
				    if (_pacman.Eaten == Food.PACGUM)
				    {
					    if (_level <= 17 || _level == 19)
					    {
						    foreach (Ghost g in _ghosts)
						    {
							    g.Mode = GhostMode.FRIGHTENED;
						    }
						    _pacman.Frightening = true;
						    _eatenGhosts = 0;
					    }
				    }
				    _pacman.Eaten = Food.NONE;

				    if (_outgoingCounter < _ghosts.Length - 1)
				    {
					    if( _ghosts[_outgoingCounter].Mode != GhostMode.OUTGOING && (// Attention, ceci est du code très sale !!
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
					    //if (_pause == 0)
					    //{
						    g.Update(_counter);
						    if (g.Mode == GhostMode.INCOMING)
						    {
							    g.Update(_counter);
						    }
						    else if (g.NbMovement > 1)
						    {
							    g.Update(_counter);
						    }
					    //}
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
						    _ghosts[ghostIndex].Drawable = false;
						    _ghosts[ghostIndex].Mode = GhostMode.INCOMING;
						    _ghostPoint = (int)Math.Pow(2, _eatenGhosts) * 100;
						    _score += _ghostPoint;
						    if (_eatenGhosts == 4)
						    {
							    _pacman.Frightening = false;
						    }
						    /*_ghosts[ghostIndex].Drawable = false;

						    // Comptage des fantomes pour les points
						    int nbGhosts = 0;
						    foreach (Ghost g in _ghosts)
						    {
							    if (g.Mode != GhostMode.FRIGHTENED)
							    {
								    ++nbGhosts;
							    }
						    }
						    _ghostPoint = nbGhosts * 200;
						    _score += nbGhosts * 200;*/
					    }
					    else if (mode != GhostMode.INCOMING)
					    {
						    --_life;
						    if (_life <= 0)
						    {
							    gameOver();
						    }
						    else
						    {
							    Initialize();
							    _ready = 60 * 2;
						    }
					    }
				    }
			    }

			    if (_pause > 0)
			    {
				    --_pause;
			    }
			    if (_pause == 1)
			    {
				    foreach (Ghost g in _ghosts)
				    {
					    g.Drawable = true;
				    }
			    }
			    if (_ready > 0)
			    {
				    --_ready;
			    }

			    if (prevScore / 10000 != _score / 10000)
			    {
				    ++_life;
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
                    if (_pause != 0)
                    {
                        // --- Affichage du score quand on mange un fantome --- //
                        textPos = _pacman.Position;
                        textPos.X -= _map.TileSize.X;
                        textPos.Y -= _map.TileSize.Y / 2;
                        //_spriteBatch.DrawString(_scoreFont, _ghostPoint.ToString(), textPos, Color.Cyan);
                        _spriteBatch.DrawString(_font, _ghostPoint.ToString(), textPos, Color.Cyan, 0f, new Vector2(0, 0), 0.6f, new SpriteEffects(), 1f);
                    }
                    else
                    {
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
                if (_ready >= 60 * 2)
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
                    _spriteBatch.Draw(_lifeTexture, lifePos, lifeClipping, Color.White);
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


			//Exit();
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

        public SpriteBatch SpriteBatch
        {
            get
            {
                return _spriteBatch;
            }
        }
	}
}
