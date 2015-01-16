using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DPixelCollisionDetection
{
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        public SpriteFont font;
        public Texture2D background, maze;
        public Texture2D ball, exit, cursor;
        public Vector2 _ballPosition, _cursorPosition;

        public bool win, reset = false;
        Rectangle rectangleA, rectangleB, rectangleC;
        Color[] dataA, dataB, dataC;
        

        public Game1() : base()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.PreferredBackBufferWidth = 800;
            Content.RootDirectory = "Content";            
        }
        protected override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("font");
            background = Content.Load<Texture2D>("greenBackground");
            maze = Content.Load<Texture2D>("maze");
            ball = Content.Load<Texture2D>("orangeBall");
            exit = Content.Load<Texture2D>("Exit");
            cursor = Content.Load<Texture2D>("cursor");
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            MouseState mouseState = Mouse.GetState();
            if(Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (win == false)
                {
                    _cursorPosition.X = mouseState.X;
                    _cursorPosition.Y = mouseState.Y;
                    _ballPosition.X = _cursorPosition.X - 50;
                    _ballPosition.Y = _cursorPosition.Y - 50;
                }
                if(reset == true)
                {
                    _ballPosition.X = 50;
                    _ballPosition.Y = 25;
                }
            }
            else
            {
                _cursorPosition.X = mouseState.X;
                _cursorPosition.Y = mouseState.Y;
                _ballPosition.X = 50;
                _ballPosition.Y = 25;
            }

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            if(win == false)
            {
                DrawGame();
                win = checkCollisionExit(win);
                reset = checkCollisionMaze(reset);
            }      
            if(reset == true)
            {
                DrawGame();
            }
            
            base.Draw(gameTime);
        }
        public void DrawHUD()
        {
            if(reset == false)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(font, "reset = false", new Vector2(0, 200), Color.White);
                _spriteBatch.End();
            }
            else
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(font, "reset = true", new Vector2(0, 200), Color.White);
                _spriteBatch.End();
            }

            if (win == false)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(font, "win = false", new Vector2(0, 150), Color.White);
                _spriteBatch.End();
            }
            else
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(font, "win = true", new Vector2(0, 150), Color.White);
                _spriteBatch.End();
            }
        }
        public void DrawGame()
        {    
            _spriteBatch.Begin();
            _spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            _spriteBatch.Draw(maze, new Vector2(0, 0), Color.White);
            _spriteBatch.Draw(exit, new Vector2(650, 25), Color.White);
            _spriteBatch.DrawString(font, "Click and drag the ball to the exit.", new Vector2(0, 300), Color.White);
            if (reset == true)
            {
                _spriteBatch.Draw(ball, new Vector2(50, 25), Color.White);
            }
            _spriteBatch.Draw(ball, new Vector2(_ballPosition.X, _ballPosition.Y), Color.White);
            _spriteBatch.Draw(cursor, new Vector2(_cursorPosition.X, _cursorPosition.Y), Color.White);
            _spriteBatch.End();
        }
        public bool checkCollisionExit(bool win)
        {
            rectangleA = new Rectangle((int)_ballPosition.X, (int)_ballPosition.Y, ball.Height, ball.Width);
            rectangleB = new Rectangle(650, 25, exit.Height, exit.Width);
            dataA = new Color[ball.Height * ball.Width];
            dataB = new Color[exit.Height * exit.Height];
            ball.GetData(dataA);
            exit.GetData(dataB);
            
            // Find the bounds of the rectangle intersection
            int exitTop = Math.Max(rectangleA.Top, rectangleB.Top);
            int exitBottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int exitLeft = Math.Max(rectangleA.Left, rectangleB.Left);
            int exitRight = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = exitTop; y < exitBottom; y++)
            {
                for (int x = exitLeft; x < exitRight; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        _spriteBatch.Begin();
                        _spriteBatch.DrawString(font, "YOU WIN!!", new Vector2(0, 500), Color.White);
                        _spriteBatch.End();
                        return true;
                    }
                }
            }
            // No intersection found
            return false;
        }
        public bool checkCollisionMaze(bool reset)
        {
            rectangleA = new Rectangle((int)_ballPosition.X, (int)_ballPosition.Y, ball.Height, ball.Width);
            rectangleC = new Rectangle(0, 0, maze.Height, maze.Width);
            dataA = new Color[ball.Height * ball.Width];
            dataC = new Color[maze.Height * maze.Width];
            ball.GetData(dataA);
            maze.GetData(dataC);

            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleC.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleC.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleC.Left);
            int right = Math.Min(rectangleA.Right, rectangleC.Right);


            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorC = dataC[(x - rectangleC.Left) +
                                         (y - rectangleC.Top) * rectangleC.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorC.A != 0)
                    {
                        //intersection found
                        _spriteBatch.Begin();
                        _spriteBatch.DrawString(font, "MAZE COLLISIONS!!", new Vector2(0, 550), Color.White);
                        _spriteBatch.End();
                        return true;
                    }
                }
            }
            // No intersection found
            return false;
        }
    }
}
