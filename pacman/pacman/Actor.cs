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

        public Actor(Texture2D texture) : base(new Vector2(28, 28))
        {

        }

        //TODO
        /*public override void Initialize();
        //TODO
        public override void LoadContent(ContentManager content);
        //TODO
        public override void Update(GameTime gameTime);
        //TODO
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        //TODO
        public override void UnloadContent(ContentManager content);*/

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
