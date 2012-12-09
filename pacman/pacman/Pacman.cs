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
		}

		private bool move(Direction dir)
		{
			Vector2 nextPos = new Vector2();
			Vector2 testOffset = new Vector2();
			switch (dir)
			{
				case Direction.UP:
				{
					nextPos = _position - new Vector2(0, _SPEEDUNIT);
					testOffset.Y -= _spriteSize.Y / 2;
					break;
				}

				case Direction.DOWN:
				{
					nextPos = _position + new Vector2(0, _SPEEDUNIT);
					testOffset.Y += _spriteSize.Y / 2;
					break;
				}

				case Direction.LEFT:
				{
					nextPos = _position - new Vector2(_SPEEDUNIT, 0);
					testOffset.X -= _spriteSize.X / 2;
					break;
				}

				case Direction.RIGHT:
				{
					nextPos = _position + new Vector2(_SPEEDUNIT, 0);
					testOffset.X += _spriteSize.X / 2;
					break;
				}
			}
			//System.Console.WriteLine((nextPos - _position).ToString());
			System.Console.WriteLine(_map.WinToMap(_position).ToString() + "\t" + (nextPos + testOffset).ToString() + "\t" + _map.WinToMap(nextPos + testOffset).ToString());

			if (!_map.isWall(_map.WinToMap(nextPos + testOffset)))
			{
				_position = nextPos;
				return true;
			}
			return false;
		}
		
		public override void Update(int counter)
		{
			KeyboardState keyboard = Keyboard.GetState();

			if(keyboard.IsKeyDown(Keys.Up))
			{
				if (_direction == Direction.DOWN)
				{
					_direction = Direction.UP;
				}
				_nextDirection = Direction.UP;
			}
			if (keyboard.IsKeyDown(Keys.Left))
			{
				if (_direction == Direction.RIGHT)
				{
					_direction = Direction.LEFT;
				}
				_nextDirection = Direction.LEFT;
			}
			if (keyboard.IsKeyDown(Keys.Down))
			{
				if (_direction == Direction.UP)
				{
					_direction = Direction.DOWN;
				}
				_nextDirection = Direction.DOWN;
			}
			if (keyboard.IsKeyDown(Keys.Right))
			{
				if (_direction == Direction.LEFT)
				{
					_direction = Direction.RIGHT;
				}
				_nextDirection = Direction.RIGHT;
			}

			if (MustMove(counter))
			{
				if (move(_direction))
				{
					System.Console.WriteLine("move 1");
					_direction = _nextDirection;
				}
				/*
				if (_direction != _nextDirection)
				{
					System.Console.WriteLine("move 2");
					move(_nextDirection);
				}
				//*/
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
