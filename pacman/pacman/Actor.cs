using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace pacman
{
    public abstract class Actor
    {
        protected int _px;
        public int Px
        {
            get;
            set;
        }

        public int X
        {
            get { return _px / _texture.Width; }
        }

        public int Y
        {
            get { return _py / _texture.Height; }
        }

        protected int _py;
        public int Py
        {
            get;
            set;
        }

        protected Texture2D _texture;
        public Texture2D Texture
        {
            get;
            set;
        }

        protected Direction _direction;
        public Direction Direction
        {
            get;
            set;
        }

        protected Map _map;

        public Actor(Map map)
        {
            _map = map;
        }

        abstract public void move();
    }
}
