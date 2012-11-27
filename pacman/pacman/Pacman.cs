using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace pacman
{/*
    public class Pacman : Actor
    {
        private Direction _nextDirection;

        public Pacman() : base()
        {
            Initialize();
        }

        //TODO
        public override void Initialize()
        {
            _direction = Direction.LEFT;
            _nextDirection = Direction.LEFT;
        }
        
        public override void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("pacman.png");
        }
        //TODO
        public override void Update(GameTime gameTime)
        {
            return;
        }
        //TODO
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            return;
        }
        //TODO
        public override void UnloadContent(ContentManager content)
        {
            return;
        }
        
        public Direction NextDirection
        {
            get { return _nextDirection; }
            set { _nextDirection = value; }
        }
        
        public void move()
        {
            if (_direction != _nextDirection)
            {
                switch (_nextDirection)
                {
                    case Direction.LEFT:
                    {
                        --_px;
                        break;
                    }
                    case Direction.RIGHT:
                    {
                        ++_px;
                        break;
                    }
                    case Direction.UP:
                    {
                        --_py;
                        break;
                    }
                    case Direction.DOWN:
                    {
                        ++_py;
                        break;
                    }
                }
            }

            switch (_direction)
            {
                case Direction.LEFT:
                {
                    --_px;
                    break;
                }
                case Direction.RIGHT:
                {
                    ++_px;
                    break;
                }
                case Direction.UP:
                {
                    --_py;
                    break;
                }
                case Direction.DOWN:
                {
                    ++_py;
                    break;
                }
            }
        }
    }
    // */
}
