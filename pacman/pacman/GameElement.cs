using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace pacman
{

	/// <summary>
	/// Abstract class provide a logic to manage any drawable elements.
	/// </summary>
	public abstract class GameElement
    {
        protected Texture2D[] _texture;
        protected int _textureIndex;
		protected Vector2 _spriteSize;
		protected int _drawCounter;
		protected int _blinkInterval;

		/// <summary>
		/// Constructor of GameElement.
		/// </summary>
		/// <param name="spriteSize">Size of the sprite.</param>
		public GameElement(Vector2 spriteSize)
		{
			this._spriteSize = spriteSize;
            _textureIndex = 0;
			//Initialize();
		}

		/// <summary>
		/// Reimplements to initialize the game element.
		/// </summary>
		abstract public void Initialize();

		/// <summary>
		/// Reimplements to load the content of game element.
		/// </summary>
		/// <param name="content">The content manager which manage the loading.</param>
		abstract public void LoadContent(ContentManager content);

		/// <summary>
		/// Reimplements to update the game element.
		/// </summary>
		/// <param name="counter">This is the main loop counter. Each increment this counter and he count from 0 to 59 (60 images per second).</param>
		abstract public void Update(int counter);

		/// <summary>
		/// Reimplements to draw the game element.
		/// </summary>
		/// <param name="spriteBatch">Allows to draw the game element.</param>
		abstract public void Draw(SpriteBatch spriteBatch);

		/// <summary>
		/// texture of the game element.
		/// </summary>
		public Texture2D Texture
		{
			get { return _texture[_textureIndex]; }
            //set { _texture[_textureIndex] = value; }
		}


		/// <summary>
		/// Size of the tile of the game element.
		/// </summary>
		public Vector2 TileSize
		{
			get { return _spriteSize; }
			set { _spriteSize = value; }
		}

		/// <summary>
		/// Index of the texture currently using.
		/// </summary>
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
