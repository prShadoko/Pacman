using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace pacman
{
	class Blinky : Ghost
	{
		public Blinky(Map map) : base(map)
		{
			_textureOffset = new Vector2(0, 0);
		}

		public override void Initialize()
		{
			base.Initialize();


			_position = new Vector2(14 * 16 + 16 / 2, 11 * 16 + 16 / 2);
			_direction = Direction.LEFT;
		}

        public override void targeting()
        {
            switch (_mode)
            {
                case GhostMode.CHASE:
                    {
                        break;
                    }
                case GhostMode.SCATTER:
                    {
                        _target = new Vector2(0, 0);
                        break;
                    }
                case GhostMode.FRIGHT:
                    {
                        break;
                    }
            }
        }
	}
}
