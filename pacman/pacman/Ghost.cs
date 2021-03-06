﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace pacman
{
	/// <summary>
	/// Enum the different ghost mode :
	///		- CHASE : ghost chase the pacman.
	///		- SCATTER : ghost scatter in the maze.
	///		- FRIGHT : ghost is frightened by pacman.
	///		- INCOMING : ghost return in the Monster Pen after has been eaten by pacman.
	///		- OUTGOING : ghost leaves the Monster House.
	///		- HOUSE : ghost wait in the Monster House.
	/// </summary>
	public enum GhostMode
	{
		CHASE = 0,
		SCATTER = 1,
		FRIGHTENED = 2,
		INCOMING = 3,
		OUTGOING = 4,
		HOUSE = 5
	};

	/// <summary>
	/// Enum the different ghost speed :
	///		- NORM : the normal speed.
	///		- FRIGHT : the frightened speed.
	///		- TUNNEL : the speed in the tunnel.
	/// </summary>
	public enum GhostSpeed { NORM = 0, FRIGHT = 1, TUNNEL = 2 };

	/// <summary>
	/// Define the Ghost class. This class implement the logic and display of a ghost.
	/// </summary>
	public abstract class Ghost : Actor
	{
		// --- attributes --- //
		protected Pacman _pacman;

		protected Vector2 _scatterTarget = new Vector2(0, 0);

		protected Vector2 _target;
		protected GhostMode _mode;

		protected bool _canThink;   // Dans le cas ou le fantome ne bouge pas sur la 1ere frame du compteur, on l'empèche de penser
		protected bool _modeChanged;
		protected bool _isFrightened;

		private int[,] _modesTime;
		private int _indexCurrentMode;

		private int _indexModeLevel; // index relatif au niveau pour le tableau des modes

		private int _modeCounter;

		private int[] _frightModeCounters;
		private int _frightModeCounter;
		private int[] _flashesCounters;
		private int _flashesCounter;
		private int _flashOffset;

		protected int _elroySpeed; // uniquement pour blinky, mais dans l'etat actuel du code, il est plus simple de le mettre ici que de changer la structure... Dommage on pensera mieux la prochaine fois
		private int _nbMovement;

		private bool _drawable;

		// --- methods --- //
		/// <summary>
		/// Constructor of Ghost class.
		/// </summary>
		/// <param name="map">Map of the pacman game.</param>
		/// <param name="pacman">The pacman evolving in the map.</param>
		public Ghost(Map map, Pacman pacman)
			: base(map)
		{
			_pacman = pacman;
		}

		/// <summary>
		/// Allows ghost to choose a valid direction in the maze.
		/// </summary>
		private void Think()
		{
			Vector2 mapPosition = _map.WinToMap(_position);

			if (_map.IsInTunnel(mapPosition))
			{
				Speed = _speedByLevel[_indexSpeedLevel, (int)GhostSpeed.TUNNEL];

				Vector2 teleportation;
				if (_map.MustTeleport(_position, out teleportation))
				{
					_position = teleportation;
				}
			}
			else if (_mode == GhostMode.FRIGHTENED)
			{
				Speed = _speedByLevel[_indexSpeedLevel, (int)GhostSpeed.FRIGHT];
			}
			else
			{
				Speed = _speedByLevel[_indexSpeedLevel, (int)GhostSpeed.NORM] + _elroySpeed;
			}

			// on récupère un tableau de directions possibles
			Direction[] dir = _map.GetWalkableDirections(mapPosition);

			/*if (_mode == GhostMode.INCOMING && _map.WinToMap(_position) == _map.TargetIncomingMode && _thinkCounter != 0)
			{
				_direction = Direction.DOWN;
			}
			else*/
			if (_mode == GhostMode.INCOMING && _map.WinToMap(_position) == _map.Respawn)
			{
				_direction = Direction.UP;
				Mode = GhostMode.OUTGOING;
			}
			else if (dir.Length == 2 && !_modeChanged && !_map.IsInSpecialZone(mapPosition)) // 2 directions et le mode ne change pas -> pas le choix on avance.
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
			else // Plus de 2 directions ou changement de mode -> Là il faut réfléchir
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
					// si c'est la direction dans laquelle on vient, ça sert a rien de tester on y retournera pas, n'y vers le haut quand on est dans une zone spécial
					if ((Actor.ReverseDirection(_direction) != d || _modeChanged) && // Attention, condition potentiellement foireuse !
						(!(d == Direction.UP && _map.IsInSpecialZone(mapPosition)) || _isFrightened)
						)
					{
						int r = 3;

						// si la direction correspond au vecteur, au augmente le rang
						if (d == Direction.UP && vec.Y < 0 ||
							 d == Direction.DOWN && vec.Y > 0 ||
							 d == Direction.RIGHT && vec.X > 0 ||
							 d == Direction.LEFT && vec.X < 0)
						{
							--r;

							// je sais pas trop comment expliquer, demande moi que je te fasse un dessin
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
			_modeChanged = false;
		}


		/// <summary>
		/// Allows ghost to choose a valid direction in the Monster House.
		/// </summary>
		private void ThinkInHouse()
		{
			Vector2 mapPosition = _map.WinToMap(_position);

			_direction = _map.GetDirectionInHouse(mapPosition, _mode, _direction);

			if (_mode == GhostMode.OUTGOING)
			{
				if (_map.IsInSpecialZone(mapPosition) == true)
				{
					if (_isFrightened)
					{
						_mode = GhostMode.FRIGHTENED;
					}
					else
					{
						Mode = getCurrentMode();
					}

					_direction = Direction.LEFT;
					_modeChanged = false;
					_thinkCounter = (int)_map.TileSize.X / 2;
				}
			}
		}

		/// <summary>
		/// Allows ghost to choose a valid direction when he is incoming to the Monster House.
		/// </summary>
		private void ThinkToIncoming()
		{
			_direction = Direction.DOWN;
			_thinkCounter = 0;
		}

		/// <summary>
		/// Update the ghost target. It must be reimplemented by differents ghosts with their strategy.
		/// </summary>
		abstract public void targeting();

		/// <summary>
		/// Update the ghost target when ghost is in INCOMING mode.
		/// </summary>
		public void targetingIncomingMode()
		{
			_target = _map.TargetIncomingMode;
		}

		/// <summary>
		/// Update the ghost target when ghost is in FRIGHT mode.
		/// </summary>
		public void targetingFrightMode()
		{
			Direction[] directions = _map.GetWalkableDirections(_map.WinToMap(_position));
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

		/// <summary>
		/// Accessor of ghost mode.
		/// </summary>
		public GhostMode Mode
		{
			get
			{
				return _mode;
			}
			set
			{
				if (_mode != GhostMode.OUTGOING)
				{
					_modeChanged = true;
				}
				if (value == GhostMode.FRIGHTENED)
				{
					_isFrightened = true;
					_frightModeCounter = 0;
					_flashOffset = 0;

					// faire attention que _level ne depasse pas la longueur du tableau
					int lvl = _level;
					if (_level > _flashesCounters.Length - 1)
					{
						lvl = _flashesCounters.Length - 1;
					}

					_flashesCounter = _flashesCounters[lvl];
					if (_mode == GhostMode.OUTGOING || _mode == GhostMode.HOUSE)
					{
						value = _mode;
					}
				}

				_mode = value;
				//Console.WriteLine(_mode);


				if (_mode == GhostMode.SCATTER)
				{
					Speed = _speedByLevel[_indexSpeedLevel, (int)GhostSpeed.NORM] + _elroySpeed;
					targeting();
				}
				else if (_mode == GhostMode.INCOMING)
				{
					Speed = 100;
					_isFrightened = false;
					targeting();
				}
				else if (_mode == GhostMode.FRIGHTENED)
				{
					Speed = _speedByLevel[_indexSpeedLevel, (int)GhostSpeed.FRIGHT];
				}
				else //if (_mode == GhostMode.CHASE || _mode == GhostMode.HOUSE || _mode == GhostMode.INCOMING || _mode == GhostMode.OUTGOING)
				{
					Speed = _speedByLevel[_indexSpeedLevel, (int)GhostSpeed.NORM] + _elroySpeed;
				}
			}
		}

		/// <summary>
		/// Get ghost mode (SCATTER or CHASE) relative to predeterminated intervals.
		/// </summary>
		/// <returns>Ghost mode (SCATTER or CHASE).</returns>
		private GhostMode getCurrentMode()
		{
			GhostMode m;
			if (_map.IsHouse(_map.WinToMap(_position)))
			{
				m = _mode;
			}
			else if (_indexCurrentMode % 2 == 0)
			{
				m = GhostMode.SCATTER;
			}
			else
			{
				m = GhostMode.CHASE;
			}
			return m;
		}

		/// <summary>
		/// Initialize ghost.
		/// </summary>
		public override void Initialize()
		{
			_nbMovement = 0;
			_speed = 100;
			_thinkCounter = 0;
			_canThink = true;
			_modeChanged = false;
			_isFrightened = false;
			_drawable = true;
			_drawCounter = 0;
			_blinkInterval = 16;
			_mode = GhostMode.HOUSE;
			targeting();

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

			_speedByLevel = new int[4, 3] {
                {75, 50, 40},
                {85, 55, 45},
                {95, 60, 50},
                {95, 95, 50}
            };

			_elroySpeed = 0;
			Speed = _speedByLevel[_indexSpeedLevel, (int)GhostSpeed.NORM] + _elroySpeed;

			_frightModeCounters = new int[] {
				6, 5, 4, 3, 2, 5, 2, 2, 1, 5, 2, 1, 1, 3, 1, 1, 1
			};

			for (int i = 0; i < _frightModeCounters.Length; ++i)
			{
				_frightModeCounters[i] *= 60;
			}

			_frightModeCounter = 0;

			_flashesCounters = new int[] {
				5, 5, 5, 5, 5, 5, 5, 5, 3, 5, 5, 3, 3, 5, 3, 3, 3
			};
			_flashesCounter = 0;
			_flashOffset = 0;
			/*for (int i = 0; i < _flashesCounters.Length; ++i)
			{
				_flashesCounters[i] = _frightModeCounters[i] - _flashesCounters[i] * ;
			}*/
		}

		/// <summary>
		/// Update ghost with the move logic and ghost strategy.
		/// </summary>
		/// <param name="counter">The frame counter (from 0 to 60).</param>
		public override void Update(int counter)
		{
			//Console.WriteLine(_isFrightened);
			// Gestion des différents changement de modes.
			if (_mode != GhostMode.FRIGHTENED && !_isFrightened)
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
					_isFrightened = false;
					// Ca c'est vraiment degeulasse... Mais j'ai pas envie de tout recoder maintenant
					_pacman.Frightening = false;
					Mode = getCurrentMode();
				}
			}

			Vector2 targetIncomingMode = _map.MapToWin(_map.TargetIncomingMode);
			targetIncomingMode.X -= (int)_map.TileSize.X / 2;

			/*if (_thinkCounter == 0 && _canThink ||
				_canThink && _mode == GhostMode.INCOMING && (_thinkCounter == (int)_map.TileSize.X / 2 ||
												 _thinkCounter == (int)_map.TileSize.X / 2 - _SPEEDUNIT )
				)*/
			if (_canThink && _mode == GhostMode.INCOMING && _position == targetIncomingMode)
			{
				ThinkToIncoming();
			}
			else if (_canThink && _thinkCounter == 0)
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

			_nbMovement = MustMove(counter);
			if (_nbMovement > 0)
			{
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

		/// <summary>
		/// Draw ghost.
		/// </summary>
		/// <param name="spriteBatch"></param>
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
					(int)textureOffset.Y * (int)_spriteSize.Y,
					(int)_spriteSize.X,
					(int)_spriteSize.Y
				);
			}
			else if (_mode == GhostMode.FRIGHTENED || _isFrightened)
			{
				Vector2 textureOffset = new Vector2(0, 8);
				//_flashOffset = 0;

				// faire attention que _level ne depasse pas la longueur du tableau
				int lvl = _level;
				if (_level > _frightModeCounters.Length - 1)
				{
					lvl = _frightModeCounters.Length - 1;
				}

				if (_frightModeCounter >= _frightModeCounters[lvl] - (_flashesCounter+1) * 14)
				{
					if (_flashOffset == 0)
						_flashOffset = 2;
					else
						_flashOffset = 0;

					--_flashesCounter;
				}
				//Console.WriteLine(_frightModeCounter);

				clipping = new Rectangle(
					((int)textureOffset.X + _drawCounter / 4 + _flashOffset) * (int)_spriteSize.X,
					(int)textureOffset.Y * (int)_spriteSize.Y,
					(int)_spriteSize.X,
					(int)_spriteSize.Y
				);
			}
			else
			{

				clipping = new Rectangle(
					((int)_direction + (int)_textureOffset.X) * (int)_spriteSize.X,
					((int)_textureOffset.Y + _drawCounter / 4) * (int)_spriteSize.Y,
					(int)_spriteSize.X,
					(int)_spriteSize.Y
					);
			}

			Vector2 pos = _position - _spriteSize / 2;

			spriteBatch.Draw(Texture, pos, clipping, Color.White);
		}

		/// <summary>
		/// Accesssor of think counter.
		/// </summary>
		public int ThinkCounter
		{
			set { _thinkCounter = value; }
			get { return _thinkCounter; }
		}

		/// <summary>
		/// Property to get the blinky speed when the elroy speed is active (see wiki).
		/// </summary>
		public int ElroySpeed
		{
			set { _elroySpeed = value; }
		}

		/// <summary>
		/// Property to get the number of movement that the ghost make in the update.
		/// </summary>
		public int NbMovement
		{
			get { return _nbMovement; }
		}
		
		/// <summary>
		/// Allows to know if the ghost must be draw or not.
		/// </summary>
		public bool Drawable
		{
			get { return _drawable; }
			set { _drawable = value; }
		}
	}
}
