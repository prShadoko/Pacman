using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace pacman
{
    class Blinky : Ghost
    {
        public Blinky() : base ()
        {


        }

        public override void Initialize()
        {
            base.Initialize();

            // TODO: demander a la map
            _position = new Vector2(42, 26);
            _direction = Direction.LEFT;
        }

        public override void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("blinky");
        }
    }
}
