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
		protected Texture2D _texture;
		protected Vector2 _spriteSize;

		public GameElement(Vector2 spriteSize)
		{
			this._spriteSize = spriteSize;
			Initialize();
		}

		abstract public void Initialize();
		abstract public void LoadContent(ContentManager content);
		abstract public void Update(GameTime gameTime);
		abstract public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
		//abstract public void UnloadContent(ContentManager content);

		public Texture2D Texture
		{
			get { return _texture; }
			set { _texture = value; }
		}

		public Vector2 TileSize
		{
			get { return _spriteSize; }
			set { _spriteSize = value; }
		}
	}
}
