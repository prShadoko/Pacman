using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace pacman
{

	/// <summary>
	/// Manage the home screen before the beginning of the game.
	/// </summary>
    class HomeScreen : GameElement
    {
        // --- attributes --- //
        private Texture2D _mapTexture;
        private SpriteFont _font;

        private int _score;
        private int _highScore;

        // --- Methods --- //

		/// <summary>
		/// Constructor of HomeScreen.
		/// </summary>
		/// <param name="score">The player one score.</param>
		/// <param name="highScore">The highscore.</param>
        public HomeScreen(int score, int highScore)
            : base(new Vector2(448, 576))
        {
            _score = score;
            _highScore = highScore;
        }

		/// <summary>
		/// Initialize HomeScreen.
		/// </summary>
        public override void Initialize()
        {
            _drawCounter = -2;
        }

		/// <summary>
		/// Load all of the content that the HomeScreen has needed.
		/// </summary>
		/// <param name="content">The content manager used for load all the content.</param>
        public override void LoadContent(ContentManager content)
        {
            _texture = new Texture2D[3];
            _texture[0] = content.Load<Texture2D>("actorsTexture");
            _texture[1] = content.Load<Texture2D>("actorsTextureSmooth");
            _texture[2] = content.Load<Texture2D>("actorsTextureModern");
            _mapTexture = content.Load<Texture2D>("mapTexture");
            _font = content.Load<SpriteFont>("ArcadeClassic");
        }

		/// <summary>
		/// Update HomeScreen animation.
		/// </summary>
		/// <param name="counter">Loop counter (0 - 59)</param>
        public override void Update(int counter)
        {
            if(counter % 30 == 0)
            {
                ++_drawCounter;
                _drawCounter %= 40;
            }
        }

		/// <summary>
		/// Draw HomeScreen.
		/// </summary>
		/// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            int characSize = 18;
            Vector2 textPos = new Vector2(0, 0);
            Vector2 pos = new Vector2(0, 0);
            string text = "";
            Rectangle clipping;

            spriteBatch.DrawString(_font, "1UP", textPos, Color.White);

            textPos.X = _spriteSize.X / 2 - (_score.ToString().Length + 2) * characSize;
            spriteBatch.DrawString(_font, _score.ToString(), textPos, Color.White);

            textPos.X = _spriteSize.X / 2;
            spriteBatch.DrawString(_font, "HIGH", textPos, Color.White);

            textPos.X = _spriteSize.X - _highScore.ToString().Length * characSize;
            spriteBatch.DrawString(_font, _highScore.ToString(), textPos, Color.White);

            text = "CHARACTER  /  NICKNAME";
            textPos.X = _spriteSize.X * 2 / 3 - text.Length * characSize / 2;
            textPos.Y = characSize * 4;
            spriteBatch.DrawString(_font, text, textPos, Color.White);

            if (_drawCounter > 0)
            {
                text = "shadow";
                if (_drawCounter > 2)
                {
                    text += " -  -  -  - \"Blinky\"";
                }
                textPos.X = characSize * 6;
                textPos.Y = characSize * 7;
                pos.X = characSize * 3;
                pos.Y = textPos.Y;
                clipping = new Rectangle(
                        ((int)Direction.RIGHT + 0) * 28,
                        (0) * 28,
                        28,
                        28
                        );

                spriteBatch.Draw(_texture[_textureIndex], pos, clipping, Color.White);

                if (_drawCounter > 1)
                {
                    spriteBatch.DrawString(_font, text, textPos, Color.Red);
                }
            }

            if (_drawCounter > 3)
            {
                text = "speedy";
                if (_drawCounter > 5)
                {
                    text += " -  -  -  - \"Pinky\"";
                }
                textPos.X = characSize * 6;
                textPos.Y = characSize * 10;
                pos.X = characSize * 3;
                pos.Y = textPos.Y;
                clipping = new Rectangle(
                        ((int)Direction.RIGHT + 0) * 28,
                        (2) * 28,
                        28,
                        28
                        );
                spriteBatch.Draw(_texture[_textureIndex], pos, clipping, Color.White);
                if (_drawCounter > 4)
                {
                    spriteBatch.DrawString(_font, text, textPos, Color.Pink);
                }
            }

            if (_drawCounter > 6)
            {
                text = "bashful";
                if (_drawCounter > 8)
                {
                    text += "  -  -  - \"Inky\"";
                }
                textPos.X = characSize * 6;
                textPos.Y = characSize * 13;
                pos.X = characSize * 3;
                pos.Y = textPos.Y;
                clipping = new Rectangle(
                        ((int)Direction.RIGHT + 0) * 28,
                        (4) * 28,
                        28,
                        28
                        );
                spriteBatch.Draw(_texture[_textureIndex], pos, clipping, Color.White);
                if (_drawCounter > 7)
                {
                    spriteBatch.DrawString(_font, text, textPos, Color.Cyan);
                }
            }

            if (_drawCounter > 9)
            {
                text = "Pokey";
                if (_drawCounter > 11)
                {
                    text += " -  -  -  -  - \"Clyde\"";
                }
                textPos.X = characSize * 6;
                textPos.Y = characSize * 16;
                pos.X = characSize * 3;
                pos.Y = textPos.Y;
                clipping = new Rectangle(
                        ((int)Direction.RIGHT + 0) * 28,
                        (6) * 28,
                        28,
                        28
                        );
                spriteBatch.Draw(_texture[_textureIndex], pos, clipping, Color.White);
                if (_drawCounter > 10)
                {
                    spriteBatch.DrawString(_font, text, textPos, Color.Orange);
                }
            }

            if (_drawCounter > 12)
            {
                text = "10 pts";
                textPos.X = characSize * 11;
                textPos.Y = characSize * 20;
                pos.X = characSize * 9;
                pos.Y = textPos.Y + 6;
                clipping = new Rectangle(
                        (int)(16 * 12),
                        0,
                        16,
                        16);
                spriteBatch.Draw(_mapTexture, pos, clipping, Color.White);
                spriteBatch.DrawString(_font, text, textPos, Color.White);
            }
            if (_drawCounter > 13)
            {
                text = "50 pts";
                textPos.X = characSize * 11;
                textPos.Y = characSize * 22;
                pos.X = characSize * 9;
                pos.Y = textPos.Y + 6;
                clipping = new Rectangle(
                        (int)(16 * 13),
                        0,
                        16,
                        16);
                spriteBatch.Draw(_mapTexture, pos, clipping, Color.White);
                spriteBatch.DrawString(_font, text, textPos, Color.White);
            }

            if (_drawCounter > 14 && _drawCounter % 2 == 1)
            {
                text = "Press  enter";
                textPos.X = characSize * 8;
                textPos.Y = characSize * 26;
                spriteBatch.DrawString(_font, text, textPos, Color.White);
            }
            if (_drawCounter > 15)
            {
                text = "tip :  press  s  to  change  skin";
                textPos.X = characSize * 5;
                textPos.Y = characSize * 30;
                spriteBatch.DrawString(_font, text, textPos, Color.White, 0f, new Vector2(0, 0), 0.7f, new SpriteEffects(), 1f);
            }
        }

		/// <summary>
		/// Property to access to the HighScore.
		/// </summary>
        public int HighScore
        {
            set
            {
                _highScore = value;
            }
        }

		/// <summary>
		/// Property to access to the score.
		/// </summary>
        public int Score
        {
            set
            {
                _score = value;
            }
        }
    }
}
