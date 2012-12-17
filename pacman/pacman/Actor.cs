using System;
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
		protected float _speed;
		protected Map _map;
		protected int _thinkCounter; // Permet de savoir quand l'acteur est au milieu d'une case

		public Actor(Map map)
			: base(new Vector2(28, 28))
		{
			_map = map;
		}

		public bool MustMove(int counter)
		{

			if (_speed == 1f ||
				_speed == 0.95f && counter % 20 != 0 ||
				_speed == 0.90f && counter % 10 != 0 ||
				_speed == 0.85f && (counter % 6 != 0 || counter % 60 == 0) ||
				_speed == 0.80f && counter % 5 != 0 ||
				_speed == 0.75f && counter % 4 != 0 ||
				_speed == 0.55f && counter % 2 != 0 && counter % 20 != 1 ||
				_speed == 0.5f && counter % 2 != 0 ||
				_speed == 0.45f && counter % 5 != 0 && counter % 5 != 2 && counter % 20 != 19 ||
				_speed == 0.4f && counter % 5 != 0 && counter % 5 != 2
				)
			{
				return true;
			}

			return false;
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
			_texture = content.Load<Texture2D>("actorsTexture");
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

		public float Speed
		{
			get { return _speed; }
			set { _speed = value; }
		}

		public float SpeedUnit
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
	}
}
