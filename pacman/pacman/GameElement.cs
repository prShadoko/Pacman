using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace pacman
{
	public abstract class GameElement
    {
        protected Texture2D[] _texture;
        protected int _textureIndex;
		protected Vector2 _spriteSize;
		protected int _drawCounter;
		protected int _blinkInterval;

		public GameElement(Vector2 spriteSize)
		{
			this._spriteSize = spriteSize;
            _textureIndex = 0;
			//Initialize();
		}

		abstract public void Initialize();
		abstract public void LoadContent(ContentManager content);
		abstract public void Update(int counter);
		abstract public void Draw(SpriteBatch spriteBatch);
		//abstract public void UnloadContent(ContentManager content);

		public Texture2D Texture
		{
			get { return _texture[_textureIndex]; }
            //set { _texture[_textureIndex] = value; }
		}

		public Vector2 TileSize
		{
			get { return _spriteSize; }
			set { _spriteSize = value; }
		}

        public int TextureIndex
        {
            get
            {
                return _textureIndex;
            }
            set
            {
                if (value >= _texture.Length)
                {
                    //Console.WriteLine("\t"+_texture.Length);
                    _textureIndex = 0;
                }
                else
                {
                    _textureIndex = value;
                }
            }
        }
	}
}
