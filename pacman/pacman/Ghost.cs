using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace pacman
{
	public enum GhostMode {
		CHASE		= 0,
		SCATTER		= 1,
		FRIGHTENED	= 2,
		INCOMING	= 3,
		OUTGOING	= 4,
		HOUSE		= 5 };
    public enum GhostSpeed { NORM = 0, FRIGHT = 1, TUNNEL = 2 };

    abstract class Ghost : Actor
    {
        // --- attributes --- //
        protected Pacman _pacman;

        protected Vector2 _scatterTarget = new Vector2(0, 0);

        protected Vector2 _target;
        protected GhostMode _mode;

        protected bool _canThink;   // Dans le cas ou le fantome ne bouge pas sur la 1ere frame du compteur, on l'empèche de penser

        private int[,] _modesTime;
        private int _indexCurrentMode;

        private int _level; // niveau en cours
        private int _indexModeLevel; // index relatif au niveau pour le tableau des modes
        private int _indexSpeedLevel; // index relatif au niveau pour le tableau des vitesses

		private int _modeCounter;

		private int[] _frightModeCounters;
		private int _frightModeCounter;
		private int[] _flashesCounters;
		private int _flashesCounter;
		private int _flashOffset;


        private float[,] _speedByLevels;
        
		// --- methods --- //
        public Ghost(Map map, Pacman pacman)
            : base(map)
        {
            _pacman = pacman;
            //_mode = GhostMode.FRIGHTENED;
        }

        private void Think()
		{

            Vector2 mapPosition = _map.WinToMap(_position);

            if (_map.isInTunnel(mapPosition))
            {
                Speed = _speedByLevels[_indexSpeedLevel, (int)GhostSpeed.TUNNEL];

				Vector2 teleportation;
				if (_map.mustTeleport(_position, out teleportation))
				{
					_position = teleportation;
				}
            }
            else if (_mode == GhostMode.FRIGHTENED)
            {
                Speed = _speedByLevels[_indexSpeedLevel, (int)GhostSpeed.FRIGHT];
            }
            else
            {
                Speed = _speedByLevels[_indexSpeedLevel, (int)GhostSpeed.NORM];
            }

            // on récupère un tableau de directions possibles
            Direction[] dir = _map.getDirectionWalkable(mapPosition);

			/*if (_mode == GhostMode.INCOMING && _map.WinToMap(_position) == _map.TargetIncomingMode && _thinkCounter != 0)
			{
				_direction = Direction.DOWN;
			}
			else*/ if (_mode == GhostMode.INCOMING && _map.WinToMap(_position) == _map.Respawn)
			{
				_direction = Direction.UP;
				Mode = GhostMode.OUTGOING;
			}
            else if (dir.Length == 2) // 2 directions -> pas le choix on avance.
            {
                if (Array.IndexOf(dir, Actor.ReverseDirection(_direction)) == 0)
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
                if (_mode == GhostMode.CHASE || _mode == GhostMode.FRIGHTENED) // MàJ de la cible si il faut suivre le Pacman ou fuir
                {
                    targeting();
                }


                // Donc on a 
                // - La liste des directions possibles
                // - la position du fantome
                // - la position du pacman
                int ranking = 5; // On va calculer le rang des directions possibles, celle qui aura le meilleur sera choisi
                Vector2 vec = _target - mapPosition; // vecteur qui indique la direction de la cible
                Direction nextDir = _direction;


                foreach (Direction d in dir)
                {
                    // si c'est la direction dans laquelle on vient, ça sert a rien de tester on y retournera pas
                    if (!(Actor.ReverseDirection(_direction) == d))
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

		private Direction getDirectionInHouse(Vector2 coordinates, Direction dir)
		{
			Direction res = Direction.UP;

			if (dir == Direction.UP)
			{
				Vector2 testPos = coordinates;
				testPos.Y -= 1;
				if (!_map.isHouse(testPos))
				{
					res = Direction.DOWN;
				}
			}
			
			else if (dir == Direction.DOWN)
			{

				Vector2 testPos = coordinates;
				testPos.Y += 1;

				if (!_map.isHouse(testPos))
				{
					res = Direction.UP;
				}
			}
			return res;
		}

		private void ThinkInHouse()
		{
			Vector2 mapPosition = _map.WinToMap(_position);

			_direction = _map.getDirectionInHouse(mapPosition, _mode, _direction);

			if (_mode == GhostMode.OUTGOING)
			{
				if (_map.isInSpecialZone(mapPosition) == true)
				{
					Mode = getCurrentMode();
					Think();
					_thinkCounter = (int)_map.TileSize.X / 2;
					if (_direction == Direction.LEFT)
					{
						_thinkCounter -= _SPEEDUNIT;
					}
				}
			}
			/*if (_mode == GhostMode.HOUSE)
			{
				if (_direction != Direction.UP && _direction != Direction.DOWN)
				{
					_direction = Direction.UP;
				}
				// --- ------------------------------------------
				if (_direction == Direction.UP)
				{
					Vector2 testPos = mapPosition;
					testPos.Y -= 1;
					if (!_map.isHouse(testPos))
					{
						_direction = Direction.DOWN;
					}
				}

				else if (_direction == Direction.DOWN)
				{

					Vector2 testPos = mapPosition;
					testPos.Y += 1;

					if (!_map.isHouse(testPos))
					{
						_direction = Direction.UP;
					}
				}
				// --- ----------------------------------------------
			}
			else if (_mode == GhostMode.OUTGOING)
			{
				if (_map.isInSpecialZone(mapPosition) == true)
				{
					_thinkCounter = (int)_map.TileSize.X / 2;
					Mode = getCurrentMode();
				}
				else if (_map.isHouseInitPosition(mapPosition))
				{
					_direction = _map.getDirectionOfInitPosition(mapPosition);
				}
				else
				{
					_direction = getDirectionInHouse(mapPosition, _direction);
				}
			}*/

			
		}

		private void ThinkToIncoming()
		{
			_direction = Direction.DOWN;
			_thinkCounter = 0;
		}

        abstract public void targeting();

		public void targetingIncomingMode()
		{
			_target = _map.TargetIncomingMode;
		}

		public void targetingFrightMode()
		{
			Direction[] directions = _map.getDirectionWalkable(_map.WinToMap(_position));
			Random rand = new Random();
			int r = rand.Next(0, 4);
			Direction nextDirection;
			Direction originDirection = Actor.ReverseDirection(_direction);
			Direction[] directionOrder = new Direction[] { Direction.UP, Direction.RIGHT, Direction.DOWN, Direction.LEFT };

			int index = Array.IndexOf(directions, (Direction)r);
			if (index != -1 && originDirection != directions[index])
			{
				nextDirection = directions[index];
			}
			else
			{
				index = Array.IndexOf(directionOrder, (Direction)r);
				nextDirection = originDirection;

				for (int i = 0; i < 4; ++i)
				{
					++index;
					if (index % 4 == 0) index = 0;
					if (Array.IndexOf(directions, directionOrder[index]) != -1 && originDirection != directionOrder[index])
					{
						nextDirection = directionOrder[index];
						break;
					}
				}
			}
			/*while (nextDirection == originDirection)
			{
				r = rand.Next(0, 4);
				nextDirection = directions[r];
			}*/

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

        public GhostMode Mode   // the Name property
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
				if (_mode == GhostMode.SCATTER || _mode == GhostMode.INCOMING)
                {
                    Speed = _speedByLevels[_indexSpeedLevel, (int)GhostSpeed.NORM];
                    targeting();
                }
				else if (_mode == GhostMode.CHASE || _mode == GhostMode.HOUSE || _mode == GhostMode.INCOMING || _mode == GhostMode.OUTGOING)
                {
                    Speed = _speedByLevels[_indexSpeedLevel, (int)GhostSpeed.NORM];
                }
                else if (_mode == GhostMode.FRIGHTENED)
                {
                    Speed = _speedByLevels[_indexSpeedLevel, (int)GhostSpeed.FRIGHT];
					_frightModeCounter = 0;
					_flashOffset = 0;

					// faire attention que _level ne depasse pas la longueur du tableau
					int lvl = _level;
					if (_level > _flashesCounters.Length - 1)
					{
						lvl = _flashesCounters.Length - 1;
					}

					_flashesCounter = _flashesCounters[lvl];
                }
            }
        }

		private GhostMode getCurrentMode()
		{
			GhostMode m;
			if (_indexCurrentMode % 2 == 0)
			{
				m = GhostMode.SCATTER;
			}
			else
			{
				m = GhostMode.CHASE;
			}
			return m;
		}

        public override void Initialize()
        {
            _speed = 1f;
            _thinkCounter = 0;
            _canThink = true;
            _drawCounter = 0;
			_blinkInterval = 16;
			_mode = GhostMode.HOUSE;
            targeting();

            _level = 1;

            _modesTime = new int[3, 7] {
            { 420, 1200, 420, 1200, 300, 1200,  300 },
            { 420, 1200, 420, 1200, 300, 61980, 1 },
            { 420, 1200, 420, 1200, 300, 62220, 1 }
            };

            _indexCurrentMode = 0;
            for (int l = 0; l < _modesTime.GetLength(0); ++l)
            {
                for (int t = 1; t < _modesTime.GetLength(1); ++t)
                {
                    _modesTime[l, t] += _modesTime[l, t - 1];
                }
            }

			_modeCounter = 0;
            _indexSpeedLevel = 0;
            _indexModeLevel = 0;
            if (_level > 1)
            {
                ++_indexSpeedLevel;
                ++_indexModeLevel;
            }
            if (_level > 4)
            {
                ++_indexSpeedLevel;
                ++_indexModeLevel;
            }
            if (_level > 5)
            {
                ++_indexSpeedLevel;
            }

            _speedByLevels = new float[4, 3] {
                {0.75f, 0.50f, 0.40f},
                {0.85f, 0.55f, 0.45f},
                {0.95f, 0.60f, 0.50f},
                {0.95f, 0.95f, 0.50f}
            };

            Speed = _speedByLevels[_indexSpeedLevel, (int)GhostSpeed.NORM];

			_frightModeCounters = new int[] {
				6,5,4,3,2,5,2,2,1,5,2,1,1,3,1,1,1
			};

			for(int i = 0; i < _frightModeCounters.Length; ++i)
			{
				_frightModeCounters[i] *= 60;
			}
			
			_frightModeCounter = 0;

			_flashesCounters = new int[] {
				5,5,5,5,5,5,5,5,3,5,5,3,3,5,3,3,3
			};
			_flashesCounter = 0;
			_flashOffset = 0;
			/*for (int i = 0; i < _flashesCounters.Length; ++i)
			{
				_flashesCounters[i] = _frightModeCounters[i] - _flashesCounters[i] * ;
			}*/
        }

        public override void Update(int counter)
        {

            // Gestion des différents changement de modes.
			if (_mode != GhostMode.FRIGHTENED)
			{
				++_modeCounter;

				if (_indexCurrentMode < _modesTime.GetLength(1) &&
					_modesTime[_indexModeLevel, _indexCurrentMode] < _modeCounter)
				{
					++_indexCurrentMode;

					if (_mode != GhostMode.OUTGOING && _mode != GhostMode.INCOMING && _mode != GhostMode.HOUSE)
					{
						Mode = getCurrentMode();
					}
					/*if (_mode == GhostMode.SCATTER)
					{
						Mode = GhostMode.CHASE;
					}
					else if (_mode == GhostMode.CHASE)
					{
						Mode = GhostMode.SCATTER;
					}*/
				}
			}
			else
			{
				++_frightModeCounter;
				int lvl = _level;

				// faire attention que _level ne depasse pas la longueur du tableau
				if (_level > _frightModeCounters.Length - 1)
				{
					lvl = _frightModeCounters.Length - 1;
				}

				if (_frightModeCounter >= _frightModeCounters[lvl])
				{
					_frightModeCounter = 0;
					Mode = getCurrentMode();
				}
			}

			Vector2 targetIncomingMode = _map.MapToWin(_map.TargetIncomingMode);
			targetIncomingMode.X -= (int)_map.TileSize.X / 2;

			/*if (_thinkCounter == 0 && _canThink ||
				_canThink && _mode == GhostMode.INCOMING && (_thinkCounter == (int)_map.TileSize.X / 2 ||
												 _thinkCounter == (int)_map.TileSize.X / 2 - _SPEEDUNIT )
				)*/
			if (_canThink && _mode == GhostMode.INCOMING && _position == targetIncomingMode )
			{
				ThinkToIncoming();
			}
			else if(_canThink && _thinkCounter == 0 )
			{ 
				if ((int)_mode < 4)
				{
					Think();
				}
				else
				{
					ThinkInHouse();
				}
				_canThink = false;
			}

            if (MustMove(counter))
			{
				//Console.Write("ok");
                _thinkCounter += _SPEEDUNIT;
                _canThink = true;
                switch (_direction)
                {
                    case Direction.UP:
                    {
                        _position.Y -= _SPEEDUNIT;
						_thinkCounter %= (int)_map.TileSize.Y;
                        break;
                    }

                    case Direction.DOWN:
                    {
						_position.Y += _SPEEDUNIT;
						_thinkCounter %= (int)_map.TileSize.Y;
                        break;
                    }

                    case Direction.LEFT:
                    {
						_position.X -= _SPEEDUNIT;
						_thinkCounter %= (int)_map.TileSize.X;
                        break;
                    }

                    case Direction.RIGHT:
                    {
						_position.X += _SPEEDUNIT;
						_thinkCounter %= (int)_map.TileSize.X;
                        break;
                    }
                }
            }
        }

		public override void Draw(SpriteBatch spriteBatch)
		{
			Rectangle clipping;

			++_drawCounter;
			_drawCounter %= 8;

			if (_mode == GhostMode.INCOMING)
			{
				Vector2 textureOffset = new Vector2(0, 9);

				clipping = new Rectangle(
						((int)_direction + (int)textureOffset.X) * (int)_spriteSize.X,
						(0 + (int)textureOffset.Y) * (int)_spriteSize.Y,
						(int)_spriteSize.X,
						(int)_spriteSize.Y);
			}
			else if (_mode == GhostMode.FRIGHTENED)
			{
				Vector2 textureOffset = new Vector2(0, 8);
				//_flashOffset = 0;
				
				// faire attention que _level ne depasse pas la longueur du tableau
				int lvl = _level;
				if (_level > _frightModeCounters.Length - 1)
				{
					lvl = _frightModeCounters.Length - 1;
				}

				
				if (_frightModeCounter >= _frightModeCounters[lvl] - _flashesCounter * 16)
				{
					if (_flashOffset == 0)
						_flashOffset = 2;
					else
						_flashOffset = 0;
					--_flashesCounter;
				}

				clipping = new Rectangle(
						((int)textureOffset.X + _drawCounter / 4 + _flashOffset) * (int)_spriteSize.X,
						(0 + (int)textureOffset.Y) * (int)_spriteSize.Y,
						(int)_spriteSize.X,
						(int)_spriteSize.Y);
			}
			else
			{

				clipping = new Rectangle(
						((int)_direction + (int)_textureOffset.X) * (int)_spriteSize.X,
						(0 + (int)_textureOffset.Y + _drawCounter / 4) * (int)_spriteSize.Y,
						(int)_spriteSize.X,
						(int)_spriteSize.Y);
			}

			Vector2 pos = _position - _spriteSize / 2;

            spriteBatch.Draw(_texture, pos, clipping, Color.White);
        }

		public int ThinkCounter
		{
			set
			{
				_thinkCounter = value;
			}
			get
			{
				return _thinkCounter;
			}
		}	
    }
}
