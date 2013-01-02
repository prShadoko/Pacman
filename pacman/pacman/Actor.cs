﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace pacman
{
	public enum Direction { UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3 };

	public abstract class Actor : GameElement
	{
		protected const int _SPEEDUNIT = 2; // Unité de déplacement
		protected Vector2 _position;
		protected Direction _direction;
		protected Vector2 _textureOffset;   // Offset in the texture map for this actor
		protected int _speed;
		protected Map _map;
		protected int _thinkCounter; // Permet de savoir quand l'acteur est au milieu d'une case
		protected int _level; // niveau en cours
		protected int _indexSpeedLevel; // index relatif au niveau pour le tableau des vitesses
		protected int[,] _speedByLevel;

		public Actor(Map map)
			: base(new Vector2(28, 28))
		{
			_map = map;
		}

		public int MustMove(int counter)
		{
			if (_speed == 105)
			{
				if (counter % 20 == 0)
				{
					return 2;
				}
				else
				{
					return 1;
				}
			}
			else if (_speed == 100 ||
				_speed == 95 && counter % 20 != 0 ||
				_speed == 90 && counter % 10 != 0 ||
				_speed == 85 && (counter % 6 != 0 || counter % 60 == 0) ||
				_speed == 80 && counter % 5 != 0 ||
				_speed == 75 && counter % 4 != 0 ||
				_speed == 60 && counter % 2 != 0 && counter % 15 != 1 ||
				_speed == 55 && counter % 2 != 0 && counter % 20 != 1 ||
				_speed == 50 && counter % 2 != 0 ||
				_speed == 45 && counter % 5 != 0 && counter % 5 != 2 && counter % 20 != 19 ||
				_speed == 40 && counter % 5 != 0 && counter % 5 != 2
				)
			{
				return 1;
			}

			return 0;
		}

		public static Direction ReverseDirection(Direction d)
		{
			switch (d)
			{
				case Direction.UP:
				d = Direction.DOWN;
				break;
				case Direction.DOWN:
				d = Direction.UP;
				break;
				case Direction.LEFT:
				d = Direction.RIGHT;
				break;
				case Direction.RIGHT:
				d = Direction.LEFT;
				break;
			}

			return d;
		}

		public override void LoadContent(ContentManager content)
		{
           // _textureIndex = 0;
            _texture = new Texture2D[2];
            _texture[0] = content.Load<Texture2D>("actorsTexture");
            _texture[1] = content.Load<Texture2D>("actorsTextureModern");
		}

		public Vector2 Position
		{
			get { return _position; }
			set { _position = value; }
		}

		public Direction Direction
		{
			get { return _direction; }
			set { _direction = value; }
		}

		public int Speed
		{
			get { return _speed; }
			set { _speed = value; }
		}

		public int SpeedUnit
		{
			get { return _SPEEDUNIT; }
		}

		/// <summary>
		/// Setter for the map.
		/// </summary>
		public Map Map
		{
			set { _map = value; }
		}

		/// <summary>
		/// Accessor of ghost level.
		/// </summary>
		public int Level
		{
			set { _level = value; }
			get { return _level; }
		}

	}
}
