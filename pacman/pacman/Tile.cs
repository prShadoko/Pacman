using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace pacman
{
    public class Tile
    {
        private Texture2D _texture;     // sprite texture
        public Texture2D Texture
        {
            get;
            set;
        }

        public Vector2 _position;      // sprite position on screen
        public Vector2 Position
        {
            get;
            set;
        }

        private Vector2 _size;
        public Vector2 Size
        {
            get;
            set;
        }

        public Tile(Texture2D texture, Vector2 position, Vector2 size)
        {
            this._texture = texture;
            this._position = position;
            this._size = size;
        }
    }
}
