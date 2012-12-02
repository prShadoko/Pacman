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
        public Inky(Map map, Pacman pacman, Blinky blinky)
            : base(map, pacman)
		{
            _blinky = blinky;
		}

		public override void Initialize()
        {
            _textureOffset = new Vector2(0, 4);

            _scatterTarget = new Vector2(27, 31);
			_position = new Vector2(14 * 16 + 16 / 2, 11 * 16 + 16 / 2);
            _direction = Direction.LEFT;

            base.Initialize();
		}

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
                case GhostMode.FRIGHT:
                    {
                        targetingFrightMode();
                        break;
                    }
            }
        }
    }
}
