using System;
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

		public Pacman(Map map)
			: base(map)
		{
			_textureOffset = new Vector2(0, 10);
		}

		//TODO
		public override void Initialize()
		{
			// TODO: demander a la map
			_position = new Vector2(14 * 16 + 16 / 2, 23 * 16 + 16 / 2);
			_direction = Direction.LEFT;
			_nextDirection = Direction.LEFT;
			_speed = 0.80f;
			_thinkCounter = 0;
		}
		
		public override void Update(int counter)
		{
			KeyboardState keyboard = Keyboard.GetState();

			if(keyboard.IsKeyDown(Keys.Up))
			{
				if (_direction == Direction.DOWN)
				{
					_direction = Direction.UP;
					_thinkCounter = ((int)_map.TileSize.Y - _thinkCounter) % (int)_map.TileSize.Y;
					System.Console.WriteLine(TileSize);
				}
				_nextDirection = Direction.UP;
			}
			if (keyboard.IsKeyDown(Keys.Left))
			{
				if (_direction == Direction.RIGHT)
				{
					_direction = Direction.LEFT;
					_thinkCounter = ((int)_map.TileSize.X - _thinkCounter) % (int)_map.TileSize.X;
					System.Console.WriteLine(_thinkCounter);
				}
				_nextDirection = Direction.LEFT;
			}
			if (keyboard.IsKeyDown(Keys.Down))
			{
				if (_direction == Direction.UP)
				{
					_direction = Direction.DOWN;
					_thinkCounter = ((int)_map.TileSize.Y - _thinkCounter) % (int)_map.TileSize.Y;
					System.Console.WriteLine(_thinkCounter);
				}
				_nextDirection = Direction.DOWN;
			}
			if (keyboard.IsKeyDown(Keys.Right))
			{
				if (_direction == Direction.LEFT)
				{
					_direction = Direction.RIGHT;
					_thinkCounter = ((int)_map.TileSize.X - _thinkCounter) % (int)_map.TileSize.X;
					System.Console.WriteLine(_thinkCounter);
				}
				_nextDirection = Direction.RIGHT;
			}

			if (MustMove(counter))
			{
				Vector2 nextCell;
				Vector2 nextPos;


				Think(_nextDirection, out nextCell, out nextPos);

				if (_thinkCounter == 0 && !_map.isWall(nextCell))
				{
					_direction = _nextDirection;
				}

				Think(_direction, out nextCell, out nextPos);

				if (!_map.isWall(nextCell) || _thinkCounter != 0)
				{
					_thinkCounter += _SPEEDUNIT;
					_thinkCounter %= (int)_map.TileSize.X;
					_position = nextPos;
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

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			Vector2 pos = _position - _spriteSize / 2;
			Rectangle clipping = new Rectangle(
					((int)_direction + (int)_textureOffset.X) * (int)_spriteSize.X,
					(0 + (int)_textureOffset.Y) * (int)_spriteSize.Y, // TODO: use gameTime to alternate the three positions of pacman
					(int)_spriteSize.X,
					(int)_spriteSize.Y);

			spriteBatch.Draw(_texture, pos, clipping, Color.White);
		}

		public Direction NextDirection
		{
			get { return _nextDirection; }
			set { _nextDirection = value; }
		}
	}
}
