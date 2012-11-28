using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace pacman
{
    public enum GhostMode {CHASE, SCATTER, FRIGHT};

    abstract class Ghost : Actor
    {
        // --- attributes --- //
        protected Vector2 _target;
        protected GhostMode _mode = GhostMode.SCATTER;

        protected int _cpt;

        public Ghost() : base()
        {

        }

        public override void Initialize()
        {
            _position = new Vector2(0, 0);
            _direction = Direction.RIGHT;

            _cpt = 0;

        }

        // TODO:
        // public override void LoadContent(ContentManager content);

        public override void Update(GameTime gameTime)
        {
            //if(_cpt % 4 != 0)
            _position.X += 2;

            //++_cpt;
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 pos = _position - _spriteSize / 2;

            spriteBatch.Draw(_texture, pos, Color.White);
        }
        /*
        public override void UnloadContent(ContentManager content)
        {
            
        }
        //*/
        public virtual void InitializeScatterMode()
        {
            _mode = GhostMode.SCATTER;
        }
    }
}
