﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace pacman
{
	public class Pacman : Actor
	{
		private Direction _nextDirection;
		private bool _isEating;
		private bool _isFrightening;
		private Food _eaten;
		private Texture2D _deathTexture;

		public Pacman(Map map)
			: base(map)
		{
			_textureOffset = new Vector2(0, 10);
		}

		public override void Initialize()
		{
			_direction = Direction.LEFT;
			_nextDirection = Direction.LEFT;
			_speed = 0.80f;
			_blinkInterval = 8;
			_thinkCounter = (int)_map.TileSize.X / 2;
			_drawCounter = 2;
			_isEating = false;
			_isFrightening = false;
			_speedByLevel = new float[4, 2] {
                {0.80f, 0.90f},
                {0.90f, 0.95f},
                {1.00f, 1.00f},
                {0.90f, 0.90f}
            };

			if(_level == 1)
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
		}

		public override void LoadContent(ContentManager content)
		{
			base.LoadContent(content);
			//_deathTexture = content.Load<Texture2D>("pacmanDeathTexture");
		}

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

		public override void Update(int counter)
		{
			bool inTunnel = _map.isInTunnel(_map.WinToMap(_position));

			// Si Pacman est sur une gomme
			if (!inTunnel && !_isEating && _thinkCounter == 0 && _map.isGum(_map.WinToMap(_position)))
			{
				_isEating = true;
			}

			// Si Pacman doit bouger sur cette frame
			if (MustMove(counter) > 0)
			{
				// Si Pacman doit manger une gomme
				if (_isEating)
				{
					_isEating = false;
					_eaten = _map.eatGum(_map.WinToMap(_position));
					// Pacman n'avancera pas sur cette frame (cf. The Pacman Dossier)
				}
				else
				{
					Vector2 nextCell;	// Coordonnees de la prochaine case dans la direction du mouvement de Pacman
					Vector2 nextPos;	// Coordonnees de la prochaine position de Pacman
					Vector2 tp;			// Destination de la teleportation par le tunnel (s'il y a lieu)

					// Si Pacman doit se teleporter
					if (_map.mustTeleport(_position, out tp))
					{
						_position = tp;
					}

					// On reflechit pour tourner dans la nouvelle direction
					Think(_nextDirection, out nextCell, out nextPos);
					if (!inTunnel && _thinkCounter == 0 && !_map.isWall(nextCell))
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
						nextIsWall = _map.isWall(nextCell);
					}

					// Si on peut avancer
					if (!nextIsWall || _thinkCounter != 0)
					{
						// On avance
						_position = nextPos;
						// On incremente les compteurs pour la reflexion et le dessin
						_thinkCounter += _SPEEDUNIT;
						_thinkCounter %= (int)_map.TileSize.X;
						++_drawCounter;
						_drawCounter %= _blinkInterval;
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

		public override void Draw(SpriteBatch spriteBatch)
		{
			Vector2 pos = _position - _spriteSize / 2;
			int stateOffset = (4 * _drawCounter) / _blinkInterval;

			Rectangle clipping = new Rectangle(
				((stateOffset == 0 ? 0 : (int)_direction) + (int)_textureOffset.X) * (int)_spriteSize.X,
				((int)_textureOffset.Y + (stateOffset % 2 == 1 ? 1 : stateOffset)) * (int)_spriteSize.Y,
				(int)_spriteSize.X,
				(int)_spriteSize.Y);

			spriteBatch.Draw(_texture, pos, clipping, Color.White);
		}

		public void DrawInit(SpriteBatch spriteBatch)
		{
			Vector2 pos = _position - _spriteSize / 2;

			Rectangle clipping = new Rectangle(
				((int)_textureOffset.X) * (int)_spriteSize.X,
				((int)_textureOffset.Y) * (int)_spriteSize.Y,
				(int)_spriteSize.X,
				(int)_spriteSize.Y);

			spriteBatch.Draw(_texture, pos, clipping, Color.White);
		}

		public void DrawDeath(SpriteBatch spriteBatch)
		{
			/*
			Vector2 pos = _position - _spriteSize / 2;
			int stateOffset = 4 * _drawCounter / _blinkInterval;

			Rectangle clipping = new Rectangle(
				((stateOffset == 0 ? 0 : (int)_direction) + (int)_textureOffset.X) * (int)_spriteSize.X,
				((int)_textureOffset.Y + (stateOffset % 2 == 1 ? 1 : stateOffset)) * (int)_spriteSize.Y,
				(int)_spriteSize.X,
				(int)_spriteSize.Y);

			spriteBatch.Draw(_texture, pos, clipping, Color.White);
			//*/
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
	}
}
