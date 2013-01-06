using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace pacman
{
    class Inky : Ghost
    {
        // -- attributes --- //
        Blinky _blinky;

        // --- methods --- //

		/// <summary>
		/// Constructor of Inky.
		/// </summary>
		/// <param name="map">The map where Inky evolve.</param>
		/// <param name="pacman">The pacman to chase.</param>
		public Inky(Map map, Pacman pacman, Blinky blinky)
            : base(map, pacman)
		{
            _blinky = blinky;
		}

		/// <summary>
		/// Initialize Inky.
		/// </summary>
		public override void Initialize()
        {
            _textureOffset = new Vector2(0, 4);

            _scatterTarget = new Vector2(27, 31);

			_position = new Vector2(0, 0);
            _direction = Direction.LEFT;

            base.Initialize();
		}
		
		/// <summary>
		/// Define the target aim by Inky.
		/// </summary>
        public override void targeting()
        {
            switch (_mode)
            {
                case GhostMode.CHASE:
                    {
                        _target = _map.WinToMap(_pacman.Position);
                        int offset = 2;
                        switch (_pacman.Direction)
                        {
                            case Direction.UP:
                                _target.X -= offset;
                                _target.Y -= offset;
                                break;
                            case Direction.DOWN:
                                _target.Y += offset;
                                break;
                            case Direction.RIGHT:
                                _target.X += offset;
                                break;
                            case Direction.LEFT:
                                _target.X -= offset;
                                break;
                        }

                        _target += _target - _map.WinToMap(_blinky.Position);
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
