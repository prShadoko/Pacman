using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace pacman
{
    class Clyde : Ghost
    {
        // -- attributes --- //

        // --- methods --- //
        public Clyde(Map map, Pacman pacman)
            : base(map, pacman)
		{
		}

		public override void Initialize()
        {
            _textureOffset = new Vector2(0, 6);

            _scatterTarget = new Vector2(0, 31);
			//_position = new Vector2(14 * 16 + 16 / 2, 11 * 16 + 16 / 2);
			_position = new Vector2(0, 0);
            _direction = Direction.LEFT;

            base.Initialize();
		}

        public override void targeting()
        {
            switch (_mode)
            {
                case GhostMode.CHASE:
                    {
                        if (pacmanEuclideanDistance() < 8)
                        {
                            _target = _scatterTarget;
                        }
                        else
                        {
                            _target = _map.WinToMap(_pacman.Position);
                        }
                    }
                    break;
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

        public int pacmanEuclideanDistance()
        {
            Vector2 vec = _map.WinToMap(_pacman.Position - _position);

            return (int)Math.Sqrt( vec.X * vec.X + vec.Y * vec.Y );
        }
    }
}
