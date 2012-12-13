﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace pacman
{
	public class Map : GameElement
	{
		private sbyte[,] _map;
		private Texture2D _tiles;

		public Map()
			: base(new Vector2(16, 16))
		{
			Initialize();
		}

		public bool isWall(Vector2 coordinates)
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

		public Tuple<bool, bool> isCenter(Vector2 coordinates)
		{
			return new Tuple<bool, bool>(coordinates.X % TileSize.X == TileSize.X / 2, coordinates.Y % TileSize.Y == TileSize.Y / 2);
		}

		public bool isHouse(Vector2 coordinates)
		{
			try
			{
				//Console.WriteLine(_map[(int)coordinates.Y, (int)coordinates.X] < -1);
				return _map[(int)coordinates.Y, (int)coordinates.X] < -1;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return false;
			}
		}

		public bool isHouseInitPosition(Vector2 coordinates)
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

		public Direction getDirectionInHouse(Vector2 coordinates, GhostMode mode, Direction dir)
		{
			Direction res = dir;

			try
			{
				if (_map[(int)coordinates.Y, (int)coordinates.X] == -2 ||
					 mode == GhostMode.OUTGOING && ( _map[(int)coordinates.Y, (int)coordinates.X] == -8 ||
													 _map[(int)coordinates.Y, (int)coordinates.X] == -6 ))
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

		public Direction getDirectionOfInitPosition(Vector2 coordinates)
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
        public bool isInTunnel(Vector2 coordinates)
        {
            if ((int)coordinates.Y == 14 &&
                ( coordinates.X < 6 ||
                  coordinates.X > 22 ) )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Allow to know if the ghost is in the special zone. In the special zone, ghosts can't turn upward
        /// </summary>
        /// <param name="coordinates">Coordinates in the map</param>
        /// <returns>return true if the ghost is in the special zone, else false</returns>
        public bool isInSpecialZone(Vector2 coordinates)
        {
            if (coordinates.X > 10 &&
                coordinates.X < 17 &&
                ( (int)coordinates.Y == 11 ||
                  (int)coordinates.Y == 23))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Allow to know if the ghost must teleport himself
        /// </summary>
        /// <param name="coordinates">Coordinates in the map</param>
        /// <returns>return true if the ghost must teleport himself, else false</returns>
        public Vector2 mustTeleportation(Vector2 coordinates)
        {
            Vector2 result = coordinates;
            if ((int)result.X == -1 && (int)result.Y == 14)
            {
                coordinates.X = 28;
            }
            else if ((int)result.X == 28 && (int)result.Y == 14)
            {
                result.X = -1;
            }
            return result;
        }

        /// <summary>
        /// Convert window coordinates to map coordinates
        /// </summary>
        /// <param name="coordinates">Window coordinates</param>
        /// <returns>Map coordinates</returns>
		public Vector2 WinToMap(Vector2 coordinates)
		{
			return new Vector2((int)(coordinates.X / _spriteSize.X), (int)(coordinates.Y / _spriteSize.Y));
		}

        /// <summary>
        /// Convert map coordinates to window coordinates
        /// </summary>
        /// <param name="coordinates">map coordinates</param>
        /// <returns>Window coordinates</returns>
        public Vector2 MapToWin(Vector2 coordinates)
        {
            Vector2 result;
            result.X = (int)coordinates.X;
            result.Y = (int)coordinates.Y;
            return result * _spriteSize + _spriteSize / 2;
        }

        public Direction[] getDirectionWalkable(Vector2 coordinates)
        {
            List<Direction> directionWalkable = new List<Direction>();

            if (isInTunnel(coordinates))
            {
                directionWalkable.Add(Direction.RIGHT);
                directionWalkable.Add(Direction.LEFT);
                return directionWalkable.ToArray();
            }

            if (!isWall( new Vector2( coordinates.X, coordinates.Y - 1 ) ) &&
                !isInSpecialZone(coordinates))
            {
                directionWalkable.Add( Direction.UP );
            }
            if ( !isWall( new Vector2( coordinates.X + 1, coordinates.Y ) ) )
            {
                directionWalkable.Add( Direction.RIGHT );
            }
            if ( !isWall( new Vector2( coordinates.X, coordinates.Y + 1 ) ) )
            {
                directionWalkable.Add( Direction.DOWN );
            }
            if ( !isWall( new Vector2( coordinates.X - 1, coordinates.Y ) ) )
            {
                directionWalkable.Add( Direction.LEFT );
            }
            return directionWalkable.ToArray();
        }

		//TODO
		public override void Initialize()
		{
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
		//TODO
		public override void LoadContent(ContentManager content)
		{
			_texture = content.Load<Texture2D>("mapTexture");
		}
		//TODO
		public override void Update(int counter)
		{
			return;
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			for (int y = 0; y < _map.GetLength(0); ++y)
			{
				for (int x = 0; x < _map.GetLength(1); ++x)
				{

					if (_map[y, x] < 14 && _map[y, x] >= 0)
					{
						Vector2 pos = new Vector2(x, y) * _spriteSize;
						Rectangle clipping = new Rectangle(
							(int)(_spriteSize.X * _map[y, x]),
							0,
							(int)_spriteSize.X,
							(int)_spriteSize.Y);

						spriteBatch.Draw(_texture, pos, clipping, Color.White);
					}
				}
			}
		}

		public Texture2D Tiles
		{
			get { return _tiles; }
			set { _tiles = value; }
		}
	}
}
