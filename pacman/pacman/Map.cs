using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace pacman
{
	public class Map : GameElement
	{
		private sbyte[,] _map;
		private int _offset;

		private Vector2 _targetIncomingMode;
		private Vector2 _respawn;

		private Ghost[] _ghosts;

		private int _nbGum;
		private Vector2 _fruitPos;
		private Food _fruit;

		private int[] _elroyDotsLeft;

		private Texture2D[] _fruitsTexture;
		private int _fruitTextureOffset;

		public Map(Ghost[] ghosts)
			: base(new Vector2(16, 16))
		{
			_ghosts = ghosts;

			_offset = 3 * (int)TileSize.Y;

			ResetMap();

			_elroyDotsLeft = new int[] {
				20, 30, 40, 40, 40, 50, 50, 50, 60, 60, 60, 80, 80, 80, 100, 100, 100, 100, 120
			};

			_fruitPos = MapToWin(new Vector2(14, 17)) - new Vector2(TileSize.X / 2, 0);
			//Console.WriteLine("\t" + _map[23, 11]);
			//Console.WriteLine(_map[23,6]);
		}

		public void ResetMap()
		{
			_nbGum = 244;

			_map = new sbyte[31, 28] {
                { 8,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  9,  8,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  9},
                { 3, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12,  2,  3, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12,  2},
                { 3, 12,  4,  1,  1,  5, 12,  4,  1,  1,  1,  5, 12,  2,  3, 12,  4,  1,  1,  1,  5, 12,  4,  1,  1,  5, 12,  2},
                { 3, 13,  2, 14, 14,  3, 12,  2, 14, 14, 14,  3, 12,  2,  3, 12,  2, 14, 14, 14,  3, 12,  2, 14, 14,  3, 13,  2},
                { 3, 12,  6,  0,  0,  7, 12,  6,  0,  0,  0,  7, 12,  6,  7, 12,  6,  0,  0,  0,  7, 12,  6,  0,  0,  7, 12,  2},
                { 3, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12,  2},
                { 3, 12,  4,  1,  1,  5, 12,  4,  5, 12,  4,  1,  1,  1,  1,  1,  1,  5, 12,  4,  5, 12,  4,  1,  1,  5, 12,  2},
                { 3, 12,  6,  0,  0,  7, 12,  2,  3, 12,  6,  0,  0,  9,  8,  0,  0,  7, 12,  2,  3, 12,  6,  0,  0,  7, 12,  2},
                { 3, 12, 12, 12, 12, 12, 12,  2,  3, 12, 12, 12, 12,  2,  3, 12, 12, 12, 12,  2,  3, 12, 12, 12, 12, 12, 12,  2},
                {10,  1,  1,  1,  1,  5, 12,  2, 10,  1,  1,  5, 14,  2,  3, 14,  4,  1,  1, 11,  3, 12,  4,  1,  1,  1,  1, 11},
                {14, 14, 14, 14, 14,  3, 12,  2,  8,  0,  0,  7, 14,  6,  7, 14,  6,  0,  0,  9,  3, 12,  2, 14, 14, 14, 14, 14},
                {14, 14, 14, 14, 14,  3, 12,  2,  3, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,  2,  3, 12,  2, 14, 14, 14, 14, 14},
                {14, 14, 14, 14, 14,  3, 12,  2,  3, 14,  4,  1,  1, -1, -1,  1,  1,  5, 14,  2,  3, 12,  2, 14, 14, 14, 14, 14},
                { 0,  0,  0,  0,  0,  7, 12,  6,  7, 14,  2, -4, -4, -8, -8, -4, -4,  3, 14,  6,  7, 12,  6,  0,  0,  0,  0,  0},
                {14, 14, 14, 14, 14, 14, 12, 14, 14, 14,  2, -7, -3, -7, -6, -7, -5,  3, 14, 14, 14, 12, 14, 14, 14, 14, 14, 14},
                { 1,  1,  1,  1,  1,  5, 12,  4,  5, 14,  2, -2, -2, -2, -2, -2, -2,  3, 14,  4,  5, 12,  4,  1,  1,  1,  1,  1},
                {14, 14, 14, 14, 14,  3, 12,  2,  3, 14,  6,  0,  0,  0,  0,  0,  0,  7, 14,  2,  3, 12,  2, 14, 14, 14, 14, 14},
                {14, 14, 14, 14, 14,  3, 12,  2,  3, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,  2,  3, 12,  2, 14, 14, 14, 14, 14},
                {14, 14, 14, 14, 14,  3, 12,  2,  3, 14,  4,  1,  1,  1,  1,  1,  1,  5, 14,  2,  3, 12,  2, 14, 14, 14, 14, 14},
                { 8,  0,  0,  0,  0,  7, 12,  6,  7, 14,  6,  0,  0,  9,  8,  0,  0,  7, 14,  6,  7, 12,  6,  0,  0,  0,  0,  9},
                { 3, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12,  2,  3, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12,  2},
                { 3, 12,  4,  1,  1,  5, 12,  4,  1,  1,  1,  5, 12,  2,  3, 12,  4,  1,  1,  1,  5, 12,  4,  1,  1,  5, 12,  2},
                { 3, 12,  6,  0,  9,  3, 12,  6,  0,  0,  0,  7, 12,  6,  7, 12,  6,  0,  0,  0,  7, 12,  2,  8,  0,  7, 12,  2},
                { 3, 13, 12, 12,  2,  3, 12, 12, 12, 12, 12, 12, 12, 14, 14, 12, 12, 12, 12, 12, 12, 12,  2,  3, 12, 12, 13,  2},
                {10,  1,  5, 12,  2,  3, 12,  4,  5, 12,  4,  1,  1,  1,  1,  1,  1,  5, 12,  4,  5, 12,  2,  3, 12,  4,  1, 11},
                { 8,  0,  7, 12,  6,  7, 12,  2,  3, 12,  6,  0,  0,  9,  8,  0,  0,  7, 12,  2,  3, 12,  6,  7, 12,  6,  0,  9},
                { 3, 12, 12, 12, 12, 12, 12,  2,  3, 12, 12, 12, 12,  2,  3, 12, 12, 12, 12,  2,  3, 12, 12, 12, 12, 12, 12,  2},
                { 3, 12,  4,  1,  1,  1,  1, 11, 10,  1,  1,  5, 12,  2,  3, 12,  4,  1,  1, 11, 10,  1,  1,  1,  1,  5, 12,  2},
                { 3, 12,  6,  0,  0,  0,  0,  0,  0,  0,  0,  7, 12,  6,  7, 12,  6,  0,  0,  0,  0,  0,  0,  0,  0,  7, 12,  2},
                { 3, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12,  2},
                {10,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, 11}
            };
		}

		public bool IsWall(Vector2 coordinates)
		{
			try
			{
				return _map[(int)coordinates.Y, (int)coordinates.X] <= 11;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return false;
			}
		}

		public bool IsGum(Vector2 coordinates)
		{
			try
			{
				return _map[(int)coordinates.Y, (int)coordinates.X] == 12 || _map[(int)coordinates.Y, (int)coordinates.X] == 13;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return false;
			}
		}

		public Food IsEatable(Vector2 coordinates)
		{
			Food res = Food.NONE;
			if (coordinates == _fruitPos)
			{
				res = _fruit;
				_fruit = Food.NONE;
			}
			else
			{
				try
				{
					Vector2 mapCoordinates = WinToMap(coordinates);
					bool eat = false;
					if (_map[(int)mapCoordinates.Y, (int)mapCoordinates.X] == 12) // Regular gum
					{
						res = Food.GUM;
						eat = true;
					}
					else if (_map[(int)mapCoordinates.Y, (int)mapCoordinates.X] == 13) // Pac-gum
					{
						res = Food.PACGUM;
						eat = true;
					}

					if (eat)
					{
						--_nbGum;
						_map[(int)mapCoordinates.Y, (int)mapCoordinates.X] = 14;
						int lvl = _ghosts[0].Level - 1;
						if (lvl >= _elroyDotsLeft.Length)
						{
							lvl = _elroyDotsLeft.Length - 1;
						}
						if (_nbGum <= _elroyDotsLeft[lvl] / 2)
						{
							_ghosts[0].ElroySpeed = 10;
						}
						else if (_nbGum <= _elroyDotsLeft[lvl])
						{
							_ghosts[0].ElroySpeed = 5;
						}
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
			return res;
		}

		/// <summary>
		/// Check if the map has still gum.
		/// </summary>
		/// <returns>Return true if it founds gum, else return false.</returns>
		public bool IsEmpty()
		{
			return _nbGum == 0;
		}

		public bool IsHouse(Vector2 coordinates)
		{
			try
			{
				return _map[(int)coordinates.Y, (int)coordinates.X] < -1;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return false;
			}
		}

		public bool IsHouseInitPosition(Vector2 coordinates)
		{
			try
			{
				return _map[(int)coordinates.Y, (int)coordinates.X] <= -5;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return false;
			}
		}

		public Direction GetDirectionInHouse(Vector2 coordinates, GhostMode mode, Direction dir)
		{
			Direction res = dir;

			try
			{
				if (_map[(int)coordinates.Y, (int)coordinates.X] == -2 ||
					 mode == GhostMode.OUTGOING && (_map[(int)coordinates.Y, (int)coordinates.X] == -8 ||
													 _map[(int)coordinates.Y, (int)coordinates.X] == -6))
				{
					res = Direction.UP;
				}
				else if (mode == GhostMode.HOUSE && _map[(int)coordinates.Y, (int)coordinates.X] == -8 ||
													_map[(int)coordinates.Y, (int)coordinates.X] == -4)
				{
					res = Direction.DOWN;
				}
				else if (mode == GhostMode.OUTGOING && _map[(int)coordinates.Y, (int)coordinates.X] == -3)
				{
					res = Direction.RIGHT;
				}
				else if (mode == GhostMode.OUTGOING && _map[(int)coordinates.Y, (int)coordinates.X] == -5)
				{
					res = Direction.LEFT;
				}
				/*else if (mode == GhostMode.HOUSE && ( _map[(int)coordinates.Y, (int)coordinates.X] == -3 ||
													  _map[(int)coordinates.Y, (int)coordinates.X] == -5 ||
													  _map[(int)coordinates.Y, (int)coordinates.X] == -6 ))
				{
					res = dir;
				}*/
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return Direction.UP;
			}

			return res;
		}

		public Direction GetDirectionOfInitPosition(Vector2 coordinates)
		{
			try
			{
				Direction res = Direction.UP;
				if (_map[(int)coordinates.Y, (int)coordinates.X] == -5)
				{
					res = Direction.RIGHT;
				}
				else if (_map[(int)coordinates.Y, (int)coordinates.X] == -7)
				{
					res = Direction.LEFT;
				}
				/*else if (_map[(int)coordinates.Y, (int)coordinates.X] == -6)
				{
					res = Direction.UP;
				}*/
				return res;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return Direction.UP;
			}
		}

		/// <summary>
		/// Allow to know if the ghost is in the tunnel
		/// </summary>
		/// <param name="coordinates">Coordinates in the map</param>
		/// <returns>return true if the ghost is in the tunnel, else false</returns>
		public bool IsInTunnel(Vector2 coordinates)
		{
			return (int)coordinates.Y == 14 && (coordinates.X < 6 || coordinates.X > 22);
		}

		/// <summary>
		/// Allow to know if the ghost is in the special zone. In the special zone, ghosts can't turn upward
		/// </summary>
		/// <param name="coordinates">Coordinates in the map</param>
		/// <returns>return true if the ghost is in the special zone, else false</returns>
		public bool IsInSpecialZone(Vector2 coordinates)
		{
			if (coordinates.X > 10 &&
				coordinates.X < 17 &&
				((int)coordinates.Y == 11 ||
				  (int)coordinates.Y == 23))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Allow to know if the ghost must teleport himself
		/// </summary>
		/// <param name="coordinates">Coordinates in the window</param>
		/// <param name="teleportation">Destination coordinates</param>
		/// <returns>return true if the actor must teleport himself, else false</returns>
		public bool MustTeleport(Vector2 coordinates, out Vector2 teleportation)
		{
			teleportation = coordinates;
			bool boolean = false;
			if (coordinates.Y == 17.5f * TileSize.Y)
			{
				if (coordinates.X == (int)(-0.5f * TileSize.X))
				{
					teleportation.X = (int)(28.5f * TileSize.X);
					boolean = true;
				}
				else if (coordinates.X == (int)(28.5f * TileSize.X))
				{
					teleportation.X = (int)(-0.5f * TileSize.X);
					boolean = true;
				}
			}
			return boolean;
		}

		/// <summary>
		/// Convert window coordinates to map coordinates
		/// </summary>
		/// <param name="coordinates">Window coordinates</param>
		/// <returns>Map coordinates</returns>
		public Vector2 WinToMap(Vector2 coordinates)
		{
			return new Vector2((int)coordinates.X / (int)TileSize.X, ((int)coordinates.Y - _offset) / (int)TileSize.Y);
		}

		/// <summary>
		/// Convert map coordinates to window coordinates
		/// </summary>
		/// <param name="coordinates">map coordinates</param>
		/// <returns>Window coordinates</returns>
		public Vector2 MapToWin(Vector2 coordinates)
		{
			//14 * 16 + 16 / 2, 23 * 16 + 16 / 2
			Vector2 result = new Vector2((int)coordinates.X, (int)coordinates.Y);
			result = result * TileSize + TileSize / 2;
			result.Y += _offset;
			return result;
		}

		public Direction[] GetWalkableDirections(Vector2 coordinates)
		{
			List<Direction> walkableDirections = new List<Direction>();

			if (IsInTunnel(coordinates))
			{
				walkableDirections.Add(Direction.RIGHT);
				walkableDirections.Add(Direction.LEFT);
				return walkableDirections.ToArray();
			}

			if (!IsWall(new Vector2(coordinates.X, coordinates.Y - 1)) /*&&
				!isInSpecialZone(coordinates)*/)
			{
				walkableDirections.Add(Direction.UP);
			}
			if (!IsWall(new Vector2(coordinates.X + 1, coordinates.Y)))
			{
				walkableDirections.Add(Direction.RIGHT);
			}
			if (!IsWall(new Vector2(coordinates.X, coordinates.Y + 1)))
			{
				walkableDirections.Add(Direction.DOWN);
			}
			if (!IsWall(new Vector2(coordinates.X - 1, coordinates.Y)))
			{
				walkableDirections.Add(Direction.LEFT);
			}

			return walkableDirections.ToArray();
		}

		public override void Initialize()
		{
			_drawCounter = 0;
			_blinkInterval = 24;

			_targetIncomingMode = new Vector2(14, 11);
			_respawn = new Vector2(14, 14);

			_fruit = Food.NONE;
		}

		public override void LoadContent(ContentManager content)
		{
			_textureIndex = 0;
			_texture = new Texture2D[2];
			_texture[0] = content.Load<Texture2D>("mapTexture");
			_texture[1] = content.Load<Texture2D>("mapTexture");

			_fruitsTexture = new Texture2D[2];
			_fruitsTexture[0] = content.Load<Texture2D>("fruitsTexture");
			_fruitsTexture[1] = content.Load<Texture2D>("fruitsTexture");
		}

		public override void Update(int counter)
		{
			return;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			++_drawCounter;
			_drawCounter %= _blinkInterval;
			for (int y = 0; y < _map.GetLength(0); ++y)
			{
				for (int x = 0; x < _map.GetLength(1); ++x)
				{

					if ((_map[y, x] < 13 && _map[y, x] >= 0) || (_map[y, x] == 13 && 2 * _drawCounter / _blinkInterval == 0))
					{
						Vector2 pos = new Vector2(x, y) * _spriteSize;
						pos.Y += _offset;
						Rectangle clipping = new Rectangle(
							(int)(_spriteSize.X * _map[y, x]),
							0,
							(int)_spriteSize.X,
							(int)_spriteSize.Y);

						spriteBatch.Draw(Texture, pos, clipping, Color.White);
					}
				}
			}

			if (_fruit != Food.NONE)
			{
				//TODO: Sprite size is different for fruits... We should avoid to hardcode its value :/
				Rectangle clipping = new Rectangle(
					(int)(28 * _fruitTextureOffset),
					0,
					(int)28,
					(int)28);

				spriteBatch.Draw(_fruitsTexture[_textureIndex], _fruitPos - new Vector2(28, 28) / 2, clipping, Color.White);
			}
		}

		public Vector2 TargetIncomingMode
		{
			get
			{
				return _targetIncomingMode;
			}
		}

		public Vector2 Respawn
		{
			get
			{
				return _respawn;
			}
		}

		public void GrowFruit(int level)
		{
			switch (level)
			{
				case 1:
				{
					_fruit = Food.CHERRY;
					_fruitTextureOffset = 0;
					break;
				}
				case 2:
				{
					_fruit = Food.STRAWBERRY;
					_fruitTextureOffset = 1;
					break;
				}
				case 3:
				case 4:
				{
					_fruit = Food.PEACH;
					_fruitTextureOffset = 2;
					break;
				}
				case 5:
				case 6:
				{
					_fruit = Food.APPLE;
					_fruitTextureOffset = 3;
					break;
				}
				case 7:
				case 8:
				{
					_fruit = Food.POMEGRANATE;
					_fruitTextureOffset = 4;
					break;
				}
				case 9:
				case 10:
				{
					_fruit = Food.GALAXIAN;
					_fruitTextureOffset = 5;
					break;
				}
				case 11:
				case 12:
				{
					_fruit = Food.BELL;
					_fruitTextureOffset = 6;
					break;
				}
				default:
				{
					_fruit = Food.KEY;
					_fruitTextureOffset = 7;
					break;
				}
			}
		}

		public void DecayFruit()
		{
			_fruit = Food.NONE;
		}

		public int NbGum
		{
			get { return _nbGum; }
		}

		public Food Fruit
		{
			get { return _fruit; }
		}

		public Vector2 FruitPos
		{
			get { return _fruitPos; }
		}
	}
}
