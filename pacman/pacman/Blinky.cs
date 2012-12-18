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
		// -- attributes --- //

		// --- methods --- //
		public Blinky(Map map, Pacman pacman)
			: base(map, pacman)
		{

			_elroySpeed = 0f;
		}

		public override void Initialize()
		{
			base.Initialize();

			_textureOffset = new Vector2(0, 0);

			_scatterTarget = new Vector2(25, -3);
			//_position = _map.MapToWin(new Vector2(14, 11));
			_position = new Vector2(0, 0);
			_direction = Direction.LEFT;

			_mode = GhostMode.OUTGOING;
		}

		public override void targeting()
		{
			switch (_mode)
			{
				case GhostMode.CHASE:
					{
						_target = _map.WinToMap(_pacman.Position);
						break;
					}
				case GhostMode.SCATTER:
					{
						_target = _scatterTarget;
						break;
					}
				case GhostMode.FRIGHTENED:
					{
						targetingFrightMode();
						break;
					}
				case GhostMode.INCOMING:
					{
						targetingIncomingMode();
						break;
					}
			}
		}

	}
}
