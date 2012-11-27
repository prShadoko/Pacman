using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace pacman
{
    public abstract class Actor : GameElement
    {
        protected Vector2 _position;
        protected Direction _direction;
        //protected String _name;

        public Actor(Texture2D texture) : base(texture, new Vector2(28, 28))
        {

        }

        //TODO
        public void Initialize();
        //TODO
        public void LoadContent(ContentManager content);
        //TODO
        public void Update(GameTime gameTime);
        //TODO
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        //TODO
        public void UnloadContent(ContentManager content);

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
    }
}
