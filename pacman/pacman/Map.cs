﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace pacman
{
	public class Map : GameElement
	{
		private byte[,] _map;
		private Texture2D _tiles;

		public Map()
			: base(new Vector2(16, 16))
		{
			Initialize();
		}

		public bool isWall(Vector2 coordinates)
		{
			return _map[(int)coordinates.Y, (int)coordinates.X] <= 11;
		}

		public Vector2 WinToMap(Vector2 coordinates)
		{
			return coordinates / _spriteSize;
		}

        public Direction[] getDirectionWalkable(Vector2 coordinates)
        {
            List<Direction> directionWalkable = new List<Direction>();

            if (!isWall( new Vector2( coordinates.X, coordinates.Y - 1 ) ) )
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
			_map = _map = new byte[31, 28] {
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
                {14, 14, 14, 14, 14,  3, 12,  2,  3, 14,  4,  1,  1, 14, 14,  1,  1,  5, 14,  2,  3, 12,  2, 14, 14, 14, 14, 14},
                { 0,  0,  0,  0,  0,  7, 12,  6,  7, 14,  2, 14, 14, 14, 14, 14, 14,  3, 14,  6,  7, 12,  6,  0,  0,  0,  0,  0},
                {14, 14, 14, 14, 14, 14, 12, 14, 14, 14,  2, 14, 14, 14, 14, 14, 14,  3, 14, 14, 14, 12, 14, 14, 14, 14, 14, 14},
                { 1,  1,  1,  1,  1,  5, 12,  4,  5, 14,  2, 14, 14, 14, 14, 14, 14,  3, 14,  4,  5, 12,  4,  1,  1,  1,  1,  1},
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
		public override void Update(GameTime gameTime)
		{
			return;
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			for (int y = 0; y < _map.GetLength(0); ++y)
			{
				for (int x = 0; x < _map.GetLength(1); ++x)
				{

					if (_map[y, x] < 14)
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
