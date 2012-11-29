using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace pacman
{
	public class Pacman : Actor
	{
		private Direction _nextDirection;

		public Pacman(Map map)
			: base(map)
		{
			_textureOffset = new Vector2(0, 10);
			Initialize();
		}

		//TODO
		public override void Initialize()
		{
			// TODO: demander a la map
			_position = new Vector2(14 * 16 + 16 / 2, 23 * 16 + 16 / 2);
			_direction = Direction.LEFT;
			_nextDirection = Direction.LEFT;
			_speed = 0.016f;//0.25f;
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
