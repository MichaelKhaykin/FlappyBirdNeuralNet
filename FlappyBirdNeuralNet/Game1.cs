using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MichaelLibrary;
using System.Collections;
using System.Collections.Generic;

namespace FlappyBirdNeuralNet
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<Bird> birds;
        float runTimeSpeed = 1;
        Bird winBird;
        Queue<Pipe> pipequeue = new Queue<Pipe>(10);
        SpriteFont font;
        int population = 100;

        bool fast = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 1600;
            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);

            birds = new List<Bird>();
            for (int i = 0; i < population; i++)
            {
                birds.Add(new Bird(pixel, new Vector2(0, GraphicsDevice.Viewport.Height / 2), Color.White, new Vector2(50)));
            }

            font = Content.Load<SpriteFont>("Font");

            int acc = 400;
            pixel.SetData(new[] { Color.White });
            for (int i = 0; i < 10; i++)
            {
                pipequeue.Enqueue(new Pipe(pixel, acc, Color.Green, birds[0], GraphicsDevice.Viewport));
                acc += 300;
            }
        }
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Space))
            {
                runTimeSpeed = 1000;
            }
            else
            {
                runTimeSpeed = 1;   
            }
            for (int z = 0; z < runTimeSpeed; z++)
            {
                Pipe[] pipes = pipequeue.ToArray();
                Pipe nextPipe = pipes[0];

                //enabled count

                winBird = birds[0];
                float bestFitness = winBird.Fitness;
                for (int i = 0; i < birds.Count; i++)
                {
                    if (winBird.Fitness < birds[i].Fitness)
                    {
                        winBird = birds[i];
                        bestFitness = birds[i].Fitness;
                    }
                }
                for (int i = 0; i < pipequeue.Count; i++)
                {
                    if (pipes[i].TopPipe.X + pipes[i].TopPipe.Texture.Width * pipes[i].TopPipe.Scale.X > winBird.X)
                    {
                        nextPipe = pipes[i];
                        break;
                    }
                }
                for (int i = 0; i < birds.Count; i++)
                {
                    birds[i].Update(gameTime, GraphicsDevice.Viewport, nextPipe);
                    birds[i].Fitness = birds[i].X - nextPipe.TopPipe.X;

                    //remove birb if off screen
                    if (birds[i].Y <= 0 || birds[i].Y >= GraphicsDevice.Viewport.Height)
                    {
                        birds[i].Enabled = false;
                    }
                    for (int j = 0; j < pipequeue.Count; j++)
                    {
                        if (birds[i].HitBox.Intersects(pipes[j].TopPipe.HitBox) || birds[i].HitBox.Intersects(pipes[j].BottomPipe.HitBox))
                        {
                            birds[i].Enabled = false;
                        }
                    }
                }

                int count = 0;
                for (int i = 0; i < birds.Count; i++)
                {
                    if (birds[i].Enabled == true)
                    {
                        count++;
                    }
                }

                if (count == 0) //no birds enabled
                {
                    for (int i = 0; i < birds.Count; i++)
                    {
                        birds[i].Brain.Clone(winBird.Brain);
                        if (birds[i] != winBird)
                        {
                            birds[i].Brain.Mutate(.5f);
                        }
                        birds[i].Position = new Vector2(0, GraphicsDevice.Viewport.Height / 2);
                        birds[i].Reset();
                    }
                }

                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            if (runTimeSpeed == 1)
            {
                Pipe[] pipes = pipequeue.ToArray();
                for (int i = 0; i < pipequeue.Count; i++)
                {
                    pipes[i].Draw(spriteBatch); 
                }

                for (int i = 0; i < birds.Count; i++)
                {
                    birds[i].Draw(spriteBatch);
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
