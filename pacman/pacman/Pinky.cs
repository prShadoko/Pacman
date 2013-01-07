using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace pacman
{
	/// <summary>
	/// This class implements the Pinky targeting logic.
	/// </summary>
    class Pinky : Ghost
    {
        // -- attributes --- //

        // --- methods --- //
		
		/// <summary>
		/// Constructor of Pinky.
		/// </summary>
		/// <param name="map">The map where Pinky evolve.</param>
		/// <param name="pacman">The pacman to chase.</param>
        public Pinky(Map map, Pacman pacman)
            : base(map, pacman)
		{
		}

		/// <summary>
		/// Initialize Pinky.
		/// </summary>
		public override void Initialize()
        {
            _textureOffset = new Vector2(0, 2);

            _scatterTarget = new Vector2(2, -3);
			//_position = new Vector2(14 * 16 + 16 / 2, 11 * 16 + 16 / 2);
			_position = new Vector2(0, 0);
            _direction = Direction.LEFT;

            base.Initialize();
		}
		
		/// <summary>
		/// Define the target aim by Pinky.
		/// </summary>
        public override void targeting()
        {
            switch (_mode)
            {
                case GhostMode.CHASE:
                    {
                        _target = _map.WinToMap(_pacman.Position);
                        switch (_pacman.Direction)
                        {
                            case Direction.UP:
                                _target.X -= 4;
                                _target.Y -= 4;
                                break;
                            case Direction.DOWN:
                                _target.Y += 4;
                                break;
                            case Direction.RIGHT:
                                _target.X += 4;
                                break;
                            case Direction.LEFT:
                                _target.X -= 4;
                                break;
                        }
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
