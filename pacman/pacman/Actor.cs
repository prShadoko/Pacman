using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace pacman
{
	public enum Direction { UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3 };

	public abstract class Actor : GameElement
	{
		protected Vector2 _position;
		protected Direction _direction;
		protected Vector2 _textureOffset;   // Offset in the texture map for this actor
		protected float _speed;
		protected Map _map;
		//protected String _name;

		public Actor(Map map)
			: base(new Vector2(28, 28))
		{
			_map = map;
		}

		public override void LoadContent(ContentManager content)
		{
			_texture = content.Load<Texture2D>("actorsTexture");
		}

		public override void Update(GameTime gameTime)
		{
			Vector2 nextPos = new Vector2();
			Vector2 testPos = new Vector2();
			switch (_direction)
			{
				case Direction.UP:
				{
					nextPos = _position - new Vector2(0, _speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds);
					testPos = nextPos - new Vector2(0, _spriteSize.Y / 2);
					break;
				}

				case Direction.DOWN:
				{
					nextPos = _position + new Vector2(0, _speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds);
					testPos = nextPos + new Vector2(0, _spriteSize.Y / 2);
					break;
				}

				case Direction.LEFT:
				{
					nextPos = _position - new Vector2(_speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds, 0);
					testPos = nextPos - new Vector2(_spriteSize.X / 2, 0);
					break;
				}

				case Direction.RIGHT:
				{
					nextPos = _position + new Vector2(_speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds, 0);
					testPos = nextPos + new Vector2(_spriteSize.X / 2, 0);
					break;
				}
			}
			Console.WriteLine("_position: " + _position.ToString() + " ; nextPos: " + nextPos.ToString() + " ; testPos: " + testPos.ToString() + " ; WinToMap(testPos): " + _map.WinToMap(testPos).ToString());
			if (!_map.isWall(_map.WinToMap(testPos)))
			{
				_position = nextPos;
			}
		}

		public Vector2 Position
		{
			get { return _position; }
			set { _position = value; }
		}

		public Direction Direction
		{
			get { return _direction; }
			set { _direction = value; }
		}
	}
}
