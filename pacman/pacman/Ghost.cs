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

        protected Pacman _pacman;

        protected Vector2 _scatterTarget = new Vector2(0, 0);

		protected Vector2 _target;
		protected GhostMode _mode;

        protected int _thinkCounter; // Permet de savoir quand le fantome est au milieu d'une case
        protected bool _canThink;   // Dans le cas ou le fantome ne bouge pas sur la 1ere frame du compteur, on l'empèche de penser

        // --- methods --- //
		public Ghost(Map map, Pacman pacman)
			: base(map)
		{
            _pacman = pacman;
            _mode = GhostMode.CHASE;
        }

        private void Think()
        {
            // on récupère un tableau de directions possibles
            Direction[] dir = _map.getDirectionWalkable( _map.WinToMap( _position ) );

            if (dir.Length == 2) // 2 directions -> pas le choix on avance.
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
            else // Plus de 2 directions -> Là il faut réfléchir
            {
                if ( _mode == GhostMode.CHASE || _mode == GhostMode.FRIGHT ) // MàJ de la cible si il faut suivre le Pacman ou fuir
                {
                    targeting();
                }


                // Donc on a 
                // - La liste des directions possibles
                // - la position du fantome
                // - la position du pacman
                int ranking = 5; // On va calculer le rang des directions possibles, celle qui aura le meilleur sera choisi
                Vector2 vec = _target - _map.WinToMap(_position); // vecteur qui indique la direction de la cible
                Direction nextDir = _direction;


                foreach(Direction d in dir)
                {
                    // si c'est la direction dans laquelle on vient, ça sert a rien de tester on y retournera pas
                    if ( !(Actor.ReverseDirection(_direction) == d) ) 
                    {
                        int r = 3;
                        
                        // si la direction correspond au vecteur, au augmente le rang
                        if (d == Direction.UP && vec.Y < 0 ||
                             d == Direction.DOWN && vec.Y > 0 ||
                             d == Direction.RIGHT && vec.X > 0 ||
                             d == Direction.LEFT && vec.X < 0)
                        {
                            --r;

                            // je sais pas trop comment expliqué, demande moi que je te fasse un dessin
                            if (Math.Abs(vec.X) > Math.Abs(vec.Y) && (d == Direction.RIGHT || d == Direction.LEFT) ||
                            Math.Abs(vec.Y) >= Math.Abs(vec.X) && (d == Direction.UP || d == Direction.DOWN))
                            {
                                --r;

                            }
                        }
                        // je sais pas trop comment expliqué, demande moi que je te fasse un dessin
                        else if (Math.Abs(vec.X) > Math.Abs(vec.Y) && (d == Direction.RIGHT || d == Direction.LEFT) ||
                            Math.Abs(vec.Y) >= Math.Abs(vec.X) && (d == Direction.UP || d == Direction.DOWN))
                        {
                            ++r;

                        }

                        if (r < ranking)
                        {
                            ranking = r;
                            nextDir = d;
                        }
                    }
                }
                _direction = nextDir;
            }
        }

        abstract public void targeting();

        public void targetingFrightMode()
        {
            Direction[] directions = _map.getDirectionWalkable(_map.WinToMap(_position));
            Random rand = new Random();
            Direction nextDirection = directions[rand.Next(0, directions.Length)];
            Direction originDirection = Actor.ReverseDirection(_direction);

            while (nextDirection == originDirection)
            {
                int r = rand.Next(0, directions.Length);
                nextDirection = directions[r];
            }

            _target = _map.WinToMap(_position);
            switch (nextDirection)
            {
                case Direction.UP:
                    --_target.Y;
                    break;
                case Direction.DOWN:
                    ++_target.Y;
                    break;
                case Direction.LEFT:
                    --_target.X;
                    break;
                case Direction.RIGHT:
                    ++_target.X;
                    break;
            }
        }

		public override void Initialize()
		{
			_speed = 1f;
            _thinkCounter = 2;
            _canThink = false;
            _drawCounter = 0;
            _mode = GhostMode.SCATTER;
            targeting();
		}

        public override void Update(int counter)
        {
            if (_thinkCounter == 0 && _canThink)
            {
                Think();
                _canThink = false;
            }

            if (MustMove(counter))
            {
                _thinkCounter += _SPEEDUNIT;
                _canThink = true;
                switch (_direction)
                {
                    case Direction.UP:
                        {
                            _position.Y -= _SPEEDUNIT;
                            if (_thinkCounter % _map.TileSize.Y == 0) _thinkCounter = 0;
                            break;
                        }

                    case Direction.DOWN:
                        {
                            _position.Y += _SPEEDUNIT;
                            if (_thinkCounter % _map.TileSize.Y == 0) _thinkCounter = 0;
                            break;
                        }

                    case Direction.LEFT:
                        {
                            _position.X -= _SPEEDUNIT;
                            if (_thinkCounter % _map.TileSize.X == 0) _thinkCounter = 0;
                            break;
                        }

                    case Direction.RIGHT:
                        {
                            _position.X += _SPEEDUNIT;
                            if (_thinkCounter % _map.TileSize.X == 0) _thinkCounter = 0;
                            break;
                        }
                }
            }
        }

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
            ++_drawCounter;
            _drawCounter %= 8;
			Vector2 pos = _position - _spriteSize / 2;
			Rectangle clipping = new Rectangle(
                    ((int)_direction + (int)_textureOffset.X) * (int)_spriteSize.X,
                    (0 + (int)_textureOffset.Y + _drawCounter/4) * (int)_spriteSize.Y, 
					(int)_spriteSize.X,
					(int)_spriteSize.Y);

			spriteBatch.Draw(_texture, pos, clipping, Color.White);
		}
	}
}
