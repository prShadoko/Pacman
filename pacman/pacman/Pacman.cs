using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace pacman
{
    public class Pacman : Actor
    {
        private Direction _nextDirection;
        public Direction NextDirection
        {
            get;
            set;
        }

        public Pacman(Map map) : base(map)
        {
            _direction = Direction.LEFT;
            _nextDirection = Direction.LEFT;
        }

        public override void move()
        {
            if (_direction != _nextDirection)
            {
                switch (_nextDirection)
                {
                    case Direction.LEFT:
                    {
                        --_x;
                        break;
                    }
                    case Direction.RIGHT:
                    {
                        ++_x;
                        break;
                    }
                    case Direction.UP:
                    {
                        --_y;
                        break;
                    }
                    case Direction.DOWN:
                    {
                        ++_y;
                        break;
                    }
                }
            }

            switch (_direction)
            {
                case Direction.LEFT:
                {
                    --_x;
                    break;
                }
                case Direction.RIGHT:
                {
                    ++_x;
                    break;
                }
                case Direction.UP:
                {
                    --_y;
                    break;
                }
                case Direction.DOWN:
                {
                    ++_y;
                    break;
                }
            }
        }
    }
}
