using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MichaelLibrary;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.IO;

namespace FlappyBirdNeuralNet
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<Bird> birds;
        float runTimeSpeed = 1;
        Bird winBird;
        Queue<Pipe> pipeQueue;
        SpriteFont font;
        int population = 100;
        Texture2D pixel;
        bool fast = false;
        int Generation = 0;
        Random random;
        int randomseed;
        int HighScore = 0;
        float bestFitness;
        KeyboardState lastKeyboard;

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

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            randomseed = Guid.NewGuid().GetHashCode();
            random = new Random(randomseed);

            Texture2D birdTexture = Content.Load<Texture2D>("coolbird");

            birds = new List<Bird>();
            for (int i = 0; i < population; i++)
            {
                birds.Add(new Bird(birdTexture, new Vector2(0 + birdTexture.Width / 2, random.Next(0, GraphicsDevice.Viewport.Height - 50)), Color.White, new Vector2(1)));
            }

            resetPipes();
        }

        private void resetPipes()
        {
            random = new Random(randomseed);

            pipeQueue = new Queue<Pipe>();

            font = Content.Load<SpriteFont>("Font");

            int acc = 400;
            pixel.SetData(new[] { Color.White });
            for (int i = 0; i < 5; i++)
            {
                pipeQueue.Enqueue(new Pipe(pixel, acc, Color.Green, birds[0].Texture.Height * birds[0].Scale.Y, GraphicsDevice.Viewport, random));
                acc += 300;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.S) && lastKeyboard.IsKeyUp(Keys.S))
            {
                var JeffString = JsonConvert.SerializeObject(winBird.Brain);
                System.IO.File.WriteAllText("JeffTheWinBird.json", JeffString);
            }
            if (keyboard.IsKeyDown(Keys.L) && lastKeyboard.IsKeyUp(Keys.L))
            {
                var JeffString = File.ReadAllText("JeffTheWinBird.json");

                for (int i = 0; i < birds.Count; i++)
                {
                    if (birds[i].Enabled)
                    {
                        birds[i].Brain = JsonConvert.DeserializeObject<NeuralNetwork>(JeffString);
                    }
                }
            }

            if (keyboard.IsKeyDown(Keys.Space))
            {
                runTimeSpeed = 100;
            }
            else
            {
                runTimeSpeed = 1;
            }

            for (int z = 0; z < runTimeSpeed; z++)
            {
                Pipe closestPipe = pipeQueue.Peek();
                foreach (var pipe in pipeQueue)
                {
                    pipe.Update(gameTime, 2f);
                }
                //enabled count
                winBird = birds[0];
                //Find best bird
                bestFitness = winBird.Fitness.Value;
                for (int i = 0; i < birds.Count; i++)
                {
                    if (winBird.Fitness.Value < birds[i].Fitness.Value)
                    {
                        winBird = birds[i];
                        bestFitness = birds[i].Fitness.Value;
                    }
                }
                if (closestPipe.TopPart.Position.X + closestPipe.TopPart.Texture.Width * closestPipe.TopPart.Scale.X < 0)
                {
                    HighScore++;
                    pipeQueue.Dequeue();
                    Pipe pipe = pipeQueue.ToArray().Last();
                    pipeQueue.Enqueue(new Pipe(pixel, (int)(pipe.TopPart.X + 300), Color.Green, birds[0].Texture.Height * birds[0].Scale.Y, GraphicsDevice.Viewport, random));
                    for (int b = 0; b < birds.Count; b++)
                    {
                        if (birds[b].Enabled)
                        {
                            birds[b].Fitness.PipesCleared++;
                        }
                    }
                }
                for (int i = 0; i < birds.Count; i++)
                {
                    if (birds[i].Enabled)
                    {
                        birds[i].Color = Color.White;
                        birds[i].Update(gameTime, GraphicsDevice.Viewport, closestPipe);
                        birds[i].Fitness.DistanceToNextGap = new Vector2(closestPipe.TopPart.X - birds[i].X, Math.Abs(closestPipe.GapY - birds[i].Y));
                    }

                    //remove birb if off screen
                    if (birds[i].Y <= 0 || birds[i].Y >= GraphicsDevice.Viewport.Height)
                    {
                        birds[i].Enabled = false;
                    }
                    foreach(var pipe in pipeQueue)
                    { 
                        if (birds[i].HitBox.Intersects(pipe.TopPart.HitBox) || birds[i].HitBox.Intersects(pipe.BottomPart.HitBox))
                        {
                            birds[i].Enabled = false;
                        }
                    }

                    winBird.Color = Color.Green;                    
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
                        birds[i].Position = new Vector2(0 + birds[i].Texture.Width / 2, GraphicsDevice.Viewport.Height / 2);
                        birds[i].Reset();
                    }
                    Generation++;
                    HighScore = 0;
                    resetPipes();
                }

                lastKeyboard = keyboard;
                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            if (runTimeSpeed == 1)
            {
                Pipe[] pipes = pipeQueue.ToArray();
                for (int i = 0; i < pipeQueue.Count; i++)
                {
                    pipes[i].Draw(spriteBatch);
                }
                for (int i = 0; i < birds.Count; i++)
                {
                    if (birds[i].Enabled && birds[i] != winBird)
                    {
                        birds[i].Draw(spriteBatch);
                    }
                }
                winBird.Draw(spriteBatch);
                spriteBatch.DrawString(font, $"Generation:{Generation}, Fitness:{bestFitness}", new Vector2(800, 0), Color.Red);
                spriteBatch.DrawString(font, $"HighScore: {HighScore}", new Vector2(800, 100), Color.Red);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
