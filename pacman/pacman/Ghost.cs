using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace pacman
{
	public enum GhostMode { CHASE, SCATTER, FRIGHT };

	abstract class Ghost : Actor
	{
		// --- attributes --- //
		protected Vector2 _target;
		protected GhostMode _mode = GhostMode.SCATTER;

		protected int _cpt;

		public Ghost(Map map)
			: base(map)
		{

		}

		public override void Initialize()
		{
			_cpt = 0;
			_speed = 0.20f;
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			Vector2 pos = _position - _spriteSize / 2;
			Rectangle clipping = new Rectangle(
					((int)_direction + (int)_textureOffset.X) * (int)_spriteSize.X,
					(0 + (int)_textureOffset.Y) * (int)_spriteSize.Y, // TODO: use gameTime to alternate the two positions of the ghost
					(int)_spriteSize.X,
					(int)_spriteSize.Y);

			spriteBatch.Draw(_texture, pos, clipping, Color.White);
		}

		public virtual void InitializeScatterMode()
		{
			_mode = GhostMode.SCATTER;
		}
	}
}
