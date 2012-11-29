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
		protected GhostMode _mode;

        protected int _thinkCounter; // Permet de savoir quand le fantome est au milieu d'une case
        protected bool _canThink;   // Dans le cas ou le fantome ne bouge pas sur la 1ere frame du compteur, on l'empèche de penser

        public const int SPEEDUNIT = 2; // Unité de déplacement

        // --- methods --- //
		public Ghost(Map map)
			: base(map)
		{

        }

        public bool MustMove(int counter)
        {
            if (_speed == 1f ||
                _speed == 0.90f && counter % 10 != 0 ||
                _speed == 0.85f && ( counter % 6 != 0 || counter % 60 == 0) ||
                _speed == 0.80f && counter % 5 != 0 ||
                _speed == 0.75f && counter % 4 != 0 ||
                _speed == 0.5f && counter % 2 != 0
                )
            {
                return true;
            }

            return false;
        }

        private void Think()
        {
            Direction[] dir = _map.getDirectionWalkable( _map.WinToMap( _position ) );

            if (dir.Length == 2)
            {
                if (Array.IndexOf(dir, Actor.ReverseDirection( _direction ) ) == 0)
                {
                    _direction = dir[1];
                }
                else
                {
                    _direction = dir[0];
                }
            }
            else
            {
                if (_mode == GhostMode.CHASE)
                {
                    targeting();
                }
                if (Array.IndexOf(dir, Actor.ReverseDirection(_direction)) == 0) //TODO !
                {
                    _direction = dir[1];
                }
                else
                {
                    _direction = dir[0];
                }
            }
        }

        abstract public void targeting();

		public override void Initialize()
		{
			_speed = 1f;
            _thinkCounter = 2;
            _canThink = false;
            _mode = GhostMode.SCATTER;
            targeting();
		}

        public void Update(GameTime gameTime, int counter)
        {
            if (_thinkCounter == 0 && _canThink)
            {
                Think();
                _canThink = false;
            }

            if (MustMove(counter))
            {
                _thinkCounter += SPEEDUNIT;
                _canThink = true;
                switch (_direction)
                {
                    case Direction.UP:
                        {
                            _position.Y -= SPEEDUNIT;
                            if (_thinkCounter % _map.TileSize.Y == 0) _thinkCounter = 0;
                            break;
                        }

                    case Direction.DOWN:
                        {
                            _position.Y += SPEEDUNIT;
                            if (_thinkCounter % _map.TileSize.Y == 0) _thinkCounter = 0;
                            break;
                        }

                    case Direction.LEFT:
                        {
                            _position.X -= SPEEDUNIT;
                            if (_thinkCounter % _map.TileSize.X == 0) _thinkCounter = 0;
                            break;
                        }

                    case Direction.RIGHT:
                        {
                            _position.X += SPEEDUNIT;
                            if (_thinkCounter % _map.TileSize.X == 0) _thinkCounter = 0;
                            break;
                        }
                }
            }
        }

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			Vector2 pos = _position - _spriteSize / 2;
			Rectangle clipping = new Rectangle(
                    ((int)_direction + (int)_textureOffset.X) * (int)_spriteSize.X,
					(0 + (int)_textureOffset.Y) * (int)_spriteSize.Y, 
					(int)_spriteSize.X,
					(int)_spriteSize.Y);

			spriteBatch.Draw(_texture, pos, clipping, Color.White);
		}
	}
}
