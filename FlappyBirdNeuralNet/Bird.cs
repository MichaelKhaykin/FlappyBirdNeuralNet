using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MichaelLibrary;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlappyBirdNeuralNet
{
    public struct Fitness
    {
        public Vector2 DistanceToNextGap { get; set; }
        public int PipesCleared { get; set; }
        public int Value
        {
            get
            {
                return (int)(1000 - DistanceToNextGap.X - DistanceToNextGap.Y + PipesCleared * 1000);
            }
        }
    }
    
    public class Bird : Sprite
    {
        public NeuralNetwork Brain;

        float speed = 0;
        float accleration = .5f;
        float initialSpeed = 6;

        public Fitness Fitness;

        public bool Enabled = true;

        public Bird(Texture2D texture, Vector2 position, Color color, Vector2 scale, Texture2D pixel = null) 
            : base(texture, position, color, scale, pixel)
        {
            Brain = new NeuralNetwork(new [] { 2, 3, 1 });
        }

        public void Reset()
        {
            speed = 0;
            accleration = .5f;
            initialSpeed = 8;
            Enabled = true;
            Fitness = default(Fitness);
        }
        public void Update(GameTime gameTime, Viewport screen, Pipe closestPipe)
        {
            if (!Enabled) { return; }

            KeyboardState keyboard = Keyboard.GetState();
            //X++ 

            speed -= accleration;
            Y -= speed;

            //calculate input

            //x distance to next pipe
            //y distance to gap in next pipe

            //scale our inputs to be the same range as our output
            List<float> input = new List<float>(2);
            input.Add(Normalize(closestPipe.TopPart.X, 1000));
            input.Add(Normalize(closestPipe.GapY - Y, 1000));
            
            float output = Brain.Run(input)[0];

            if (output > 0.5)
            {
                speed = initialSpeed;
            }
            else
            {

            }
        }

        public float Normalize(float value, float max)
        {
            if (value < 0) value = 0;
            else if (value > max) value = max;

            return value / max; //percent value
        }
    }
}
