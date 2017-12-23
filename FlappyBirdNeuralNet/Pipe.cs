using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MichaelLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlappyBirdNeuralNet
{
    public class Pipe
    {
        int gapsize;
        public Sprite BottomPipe;
        public Sprite TopPipe;
        private int randomPlaceToPutGapPosition;
        public Rectangle HitBox { get; set; }

        public float GapY
        {
            get
            {
                return randomPlaceToPutGapPosition + gapsize / 2;
            }
        }
        public Pipe(Texture2D texture, int x, Color color, Bird bird, Viewport viewport)
        {
            //Color wil be green for pipes, no need to taken the color
            gapsize = (int)(bird.Texture.Height * bird.Scale.Y) + 100;

            int margin = 100;
            Random randomPlaceToPutGap = new Random(Guid.NewGuid().GetHashCode());
            randomPlaceToPutGapPosition = randomPlaceToPutGap.Next(margin, viewport.Height - gapsize - margin);

            Rectangle topPipeBounds = new Rectangle(x, 0, 50, randomPlaceToPutGapPosition);
            Rectangle bottomPipeBounds = new Rectangle(x, randomPlaceToPutGapPosition + gapsize, 50, viewport.Height - randomPlaceToPutGapPosition - gapsize);

            TopPipe = new Sprite(texture, new Vector2(topPipeBounds.X, topPipeBounds.Y), color, new Vector2(topPipeBounds.Width, topPipeBounds.Height));
            BottomPipe = new Sprite(texture, new Vector2(bottomPipeBounds.X, bottomPipeBounds.Y), color, new Vector2(bottomPipeBounds.Width, bottomPipeBounds.Height));
            
        }
        public void Update(GameTime gameTime, float speed)
        {
            TopPipe.X -= speed;
            BottomPipe.X -= speed;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            TopPipe.Draw(spriteBatch);
            BottomPipe.Draw(spriteBatch);
        }
    }
}
