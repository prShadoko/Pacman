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

		public Pacman(Map map)
			: base(map)
		{
			_textureOffset = new Vector2(0, 10);
		}

		public override void Initialize()
		{
			// TODO: demander a la map
			_position = new Vector2(14 * 16 + 16 / 2, 23 * 16 + 16 / 2);
			_direction = Direction.LEFT;
			_nextDirection = Direction.LEFT;
			_speed = 0.80f;
			_thinkCounter = 0;
			_drawCounter = 0;
			_blinkInterval = 8;
			_isEating = false;
		}

		public override void Update(int counter)
		{
			KeyboardState keyboard = Keyboard.GetState();

			if (keyboard.IsKeyDown(Keys.Up))
			{
				if (_direction == Direction.DOWN)
				{
					_direction = Direction.UP;
					_thinkCounter = ((int)_map.TileSize.Y - _thinkCounter) % (int)_map.TileSize.Y;
					_drawCounter = (_blinkInterval - _drawCounter) % (int)_map.TileSize.Y;
				}
				_nextDirection = Direction.UP;
			}
			if (keyboard.IsKeyDown(Keys.Left))
			{
				if (_direction == Direction.RIGHT)
				{
					_direction = Direction.LEFT;
					_thinkCounter = ((int)_map.TileSize.X - _thinkCounter) % (int)_map.TileSize.X;
					_drawCounter = (_blinkInterval - _drawCounter) % (int)_map.TileSize.X;
				}
				_nextDirection = Direction.LEFT;
			}
			if (keyboard.IsKeyDown(Keys.Down))
			{
				if (_direction == Direction.UP)
				{
					_direction = Direction.DOWN;
					_thinkCounter = ((int)_map.TileSize.Y - _thinkCounter) % (int)_map.TileSize.Y;
					_drawCounter = (_blinkInterval - _drawCounter) % (int)_map.TileSize.Y;
				}
				_nextDirection = Direction.DOWN;
			}
			if (keyboard.IsKeyDown(Keys.Right))
			{
				if (_direction == Direction.LEFT)
				{
					_direction = Direction.RIGHT;
					_thinkCounter = ((int)_map.TileSize.X - _thinkCounter) % (int)_map.TileSize.X;
					_drawCounter = (_blinkInterval - _drawCounter) % (int)_map.TileSize.X;
				}
				_nextDirection = Direction.RIGHT;
			}

			if (!_isEating && _thinkCounter == 0 && _map.isGum(_map.WinToMap(_position)))
			{
				_isEating = !_isEating;
			}


			if (MustMove(counter))
			{
				if (_isEating)
				{
					_isEating = false;
					_map.eatGum(_map.WinToMap(_position));
					//TODO: compter les points
				}
				else
				{
					Vector2 nextCell;
					Vector2 nextPos;
					Vector2 tp;
					if (_map.mustTeleport(_position, out tp))
					{
						_position = tp;
					}
					Think(_nextDirection, out nextCell, out nextPos);
					if (_thinkCounter == 0 && !_map.isWall(nextCell))
					{
						_direction = _nextDirection;
					}
					else
					{
						Think(_direction, out nextCell, out nextPos);
					}

					if (!_map.isWall(nextCell) || _thinkCounter != 0)
					{
						_thinkCounter += _SPEEDUNIT;
						_thinkCounter %= (int)_map.TileSize.X;
						_position = nextPos;
						++_drawCounter;
						_drawCounter %= _blinkInterval;
					}

				}
			}
		}

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
			int stateOffset = 4*_drawCounter / _blinkInterval;

			Rectangle clipping = new Rectangle(
				((stateOffset == 0 ? 0 : (int)_direction) + (int)_textureOffset.X) * (int)_spriteSize.X,
				((int)_textureOffset.Y + (stateOffset % 2 == 1 ? 1 : stateOffset)) * (int)_spriteSize.Y,
				(int)_spriteSize.X,
				(int)_spriteSize.Y);

			spriteBatch.Draw(_texture, pos, clipping, Color.White);
		}

		/// <summary>
		/// Accessor of the map.
		/// </summary>
		public Map Map
		{
			set
			{
				_map = value;
			}
		}
	}
}
