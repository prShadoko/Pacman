using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace pacman
{
	/// <summary>
	/// Manage the pacman played by the player.
	/// </summary>
	public class Pacman : Actor
	{
		private Direction _nextDirection;
		private bool _isEating;
		private bool _isFrightening;
		private bool _isAlive;
		private Food _eaten;
		protected Texture2D[] _deathTexture;
		private SoundEffectInstance _soundWalking;

		private Vector2 _deathSpriteSize;
		private int _deathCounter;

		/// <summary>
		/// Constructor of pacman.
		/// </summary>
		/// <param name="map">The maze where the pacman evolve.</param>
		public Pacman(Map map)
			: base(map)
		{
			_textureOffset = new Vector2(0, 10);
		}

		/// <summary>
		/// Initialize the pacman.
		/// </summary>
		public override void Initialize()
		{
			_deathCounter = 0;
			_direction = Direction.LEFT;
			_nextDirection = Direction.LEFT;
			_speed = 80;
			_blinkInterval = 8;
			_thinkCounter = (int)_map.TileSize.X / 2;
			_drawCounter = 2;
			_isEating = false;
			_isFrightening = false;
			_isAlive = true;
			_speedByLevel = new int[4, 2] {
                {80, 90},
                {90, 95},
                {100, 100},
                {90, 90}
            };

			if (_level == 1)
			{
				_indexSpeedLevel = 0;
			}
			else if (_level < 5)
			{
				_indexSpeedLevel = 1;
			}
			else if (_level < 21)
			{
				_indexSpeedLevel = 2;
			}
			else
			{
				_indexSpeedLevel = 3;
			}
			_speed = _speedByLevel[_indexSpeedLevel, 0];

			if (_soundWalking != null)
			{
				_soundWalking.Stop();
			}
		}

		/// <summary>
		/// Load the content used by pacman.
		/// </summary>
		/// <param name="content">The content manager which load the content.</param>
		public override void LoadContent(ContentManager content)
		{
			base.LoadContent(content);

			SoundEffect sound = content.Load<SoundEffect>("Wawa");
			_soundWalking = sound.CreateInstance();
			_soundWalking.IsLooped = true;
			_deathTexture = new Texture2D[3];
			_deathTexture[0] = content.Load<Texture2D>("deathTexture");
			_deathTexture[1] = content.Load<Texture2D>("deathTextureSmooth");
			_deathTexture[2] = content.Load<Texture2D>("deathTextureModern");
			_deathSpriteSize = new Vector2(42, 42);
		}

		/// <summary>
		/// Update the direction of the pacman relative to thekeyboard state.
		/// </summary>
		public void UpdateDirection()
		{
			KeyboardState keyboard = Keyboard.GetState();
			// Si le joueur appuie sur la touche haut
			if (keyboard.IsKeyDown(Keys.Up))
			{
				// Si pacman va vers le bas
				if (_direction == Direction.DOWN)
				{
					// On change instantanement de direction 
					_direction = Direction.UP;
					// On preserve une valeur coherente pour les compteurs
					_thinkCounter = ((int)_map.TileSize.Y - _thinkCounter) % (int)_map.TileSize.Y;
					_drawCounter = (_blinkInterval - _drawCounter) % _blinkInterval;
				}
				// On tournera vers le haut des que possible
				_nextDirection = Direction.UP;
			}
			if (keyboard.IsKeyDown(Keys.Down))
			{
				if (_direction == Direction.UP)
				{
					_direction = Direction.DOWN;
					_thinkCounter = ((int)_map.TileSize.Y - _thinkCounter) % (int)_map.TileSize.Y;
					_drawCounter = (_blinkInterval - _drawCounter) % _blinkInterval;
				}
				_nextDirection = Direction.DOWN;
			}
			if (keyboard.IsKeyDown(Keys.Left))
			{
				if (_direction == Direction.RIGHT)
				{
					_direction = Direction.LEFT;
					_thinkCounter = ((int)_map.TileSize.X - _thinkCounter) % (int)_map.TileSize.X;
					_drawCounter = (_blinkInterval - _drawCounter) % _blinkInterval;
				}
				_nextDirection = Direction.LEFT;
			}
			if (keyboard.IsKeyDown(Keys.Right))
			{
				if (_direction == Direction.LEFT)
				{
					_direction = Direction.RIGHT;
					_thinkCounter = ((int)_map.TileSize.X - _thinkCounter) % (int)_map.TileSize.X;
					_drawCounter = (_blinkInterval - _drawCounter) % _blinkInterval;
				}
				_nextDirection = Direction.RIGHT;
			}
		}

		/// <summary>
		/// Update pacman with the game logic.
		/// </summary>
		/// <param name="counter">Loop counter of the game.</param>
		public override void Update(int counter)
		{
			bool inTunnel = _map.IsInTunnel(_map.WinToMap(_position));

			// Si Pacman est sur une gomme ou un fruit
			if ((!inTunnel && !_isEating && _thinkCounter == 0 && _map.IsGum(_map.WinToMap(_position))) || (_map.Fruit != Food.NONE && _position == _map.FruitPos))
			{
				_isEating = true;
			}

			// Si Pacman doit bouger sur cette frame
			if (MustMove(counter) > 0)
			{
				// Si Pacman doit manger une gomme ou un fruit
				if (_isEating)
				{
					_isEating = false;
					_eaten = _map.IsEatable(_position);
					// Pacman n'avancera pas sur cette frame (cf. The Pacman Dossier)
				}
				else
				{
					Vector2 nextCell;	// Coordonnees de la prochaine case dans la direction du mouvement de Pacman
					Vector2 nextPos;	// Coordonnees de la prochaine position de Pacman
					Vector2 tp;			// Destination de la teleportation par le tunnel (s'il y a lieu)

					// Si Pacman doit se teleporter
					if (_map.MustTeleport(_position, out tp))
					{
						_position = tp;
					}

					// On reflechit pour tourner dans la nouvelle direction
					Think(_nextDirection, out nextCell, out nextPos);
					if (!inTunnel && _thinkCounter == 0 && !_map.IsWall(nextCell))
					{
						// On tourne
						_direction = _nextDirection;
					}
					else
					{
						// On reflechit pour continuer d'avancer
						Think(_direction, out nextCell, out nextPos);
					}

					// Si Pacman n'est pas dans le tunnel
					bool nextIsWall = false;
					if (!inTunnel)
					{
						// On verifie si la case suivante est un mur
						nextIsWall = _map.IsWall(nextCell);
					}

					// Si on peut avancer
					if (!nextIsWall || _thinkCounter != 0)
					{
						if (_soundWalking.State != SoundState.Playing)
						{
							_soundWalking.Play();
						}
						// On avance
						_position = nextPos;
						// On incremente les compteurs pour la reflexion et le dessin
						_thinkCounter += _SPEEDUNIT;
						_thinkCounter %= (int)_map.TileSize.X;
						++_drawCounter;
						_drawCounter %= _blinkInterval;
					}
					else
					{
						_soundWalking.Stop();
					}
				}
			}
		}

		/// <summary>
		/// Allows to know if a given direction is practicable
		/// </summary>
		/// <param name="dir">The direction to study</param>
		/// <param name="nextCell">Coordinates of the next cell in the studied direction</param>
		/// <param name="nextPos">Next Pacman position</param>
		private void Think(Direction dir, out Vector2 nextCell, out Vector2 nextPos)
		{
			nextCell = _map.WinToMap(_position);
			nextPos = _position;
			switch (dir)
			{
				case Direction.UP:
				{
					nextCell.Y--;
					nextPos.Y -= _SPEEDUNIT;
					break;
				}

				case Direction.DOWN:
				{
					nextCell.Y++;
					nextPos.Y += _SPEEDUNIT;
					break;
				}

				case Direction.LEFT:
				{
					nextCell.X--;
					nextPos.X -= _SPEEDUNIT;
					break;
				}

				case Direction.RIGHT:
				{
					nextCell.X++;
					nextPos.X += _SPEEDUNIT;
					break;
				}
			}
		}

		/// <summary>
		/// Draw the pacman on the window.
		/// </summary>
		/// <param name="spriteBatch">The spritebatch which draw the pacman.</param>
		public override void Draw(SpriteBatch spriteBatch)
		{
			if (_isAlive)
			{
				Vector2 pos = _position - _spriteSize / 2;
				int stateOffset = (4 * _drawCounter) / _blinkInterval;

				Rectangle clipping = new Rectangle(
					((stateOffset == 0 ? 0 : (int)_direction) + (int)_textureOffset.X) * (int)_spriteSize.X,
					((int)_textureOffset.Y + (stateOffset % 2 == 1 ? 1 : stateOffset)) * (int)_spriteSize.Y,
					(int)_spriteSize.X,
					(int)_spriteSize.Y);

				spriteBatch.Draw(_texture[_textureIndex], pos, clipping, Color.White);
			}
			else
			{
				Vector2 pos = _position - _deathSpriteSize / 2;
				int stateOffset = _deathCounter / 8;
				++_deathCounter;

				Rectangle clipping = new Rectangle(
					stateOffset * (int)_deathSpriteSize.X,
					0,
					(int)_deathSpriteSize.X,
					(int)_deathSpriteSize.Y);

				spriteBatch.Draw(_deathTexture[_textureIndex], pos, clipping, Color.White);
			}
		}

		/// <summary>
		/// Draw the first pacman occurence at the beginning of the game (Pacman with his closed mouth).
		/// </summary>
		/// <param name="spriteBatch">The spritebatch which draw the pacman.</param>
		public void DrawInit(SpriteBatch spriteBatch)
		{
			Vector2 pos = _position - _spriteSize / 2;

			Rectangle clipping = new Rectangle(
				((int)_textureOffset.X) * (int)_spriteSize.X,
				((int)_textureOffset.Y) * (int)_spriteSize.Y,
				(int)_spriteSize.X,
				(int)_spriteSize.Y);

			spriteBatch.Draw(_texture[_textureIndex], pos, clipping, Color.White);
		}

		/// <summary>
		/// Getter and setter for the eaten stuff
		/// </summary>
		public Food Eaten
		{
			get { return _eaten; }
			set { _eaten = value; }
		}

		/// <summary>
		/// Getter and setter for frightening mode
		/// </summary>
		public bool Frightening
		{
			get { return _isFrightening; }
			set
			{
				_isFrightening = value;
				_speed = _speedByLevel[_indexSpeedLevel, (_isFrightening ? 1 : 0)];
			}
		}

		/// <summary>
		/// Getter and setter for pacman state.
		/// </summary>
		public bool Alive
		{
			get { return _isAlive; }
			set { 
				_isAlive = value;
				if (!_isAlive)
				{
					_soundWalking.Stop();
				}
			}
		}
	}
}
