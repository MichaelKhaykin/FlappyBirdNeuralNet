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
        public Sprite BottomPart;
        public Sprite TopPart;
        private int randomPlaceToPutGapPosition;
        public Rectangle HitBox { get; set; }
        Rectangle topPipeBounds;
        Rectangle bottomPipeBounds;

        public float GapY
        {
            get
            {
                return randomPlaceToPutGapPosition + gapsize / 2;
            }
        }
        public Pipe(Texture2D texture, int x, Color color, float scaledBirdHeight, Viewport viewport, Random randomPlaceToPutGap)
        {
            //Color wil be green for pipes, no need to taken the color
            gapsize = (int)(scaledBirdHeight * 6);

            int margin = 100;
           
            randomPlaceToPutGapPosition = randomPlaceToPutGap.Next(margin, viewport.Height - gapsize - margin);

            topPipeBounds = new Rectangle(x, 0, 50, randomPlaceToPutGapPosition);
            bottomPipeBounds = new Rectangle(x, randomPlaceToPutGapPosition + gapsize, 50, viewport.Height - randomPlaceToPutGapPosition - gapsize);

            TopPart = new Sprite(texture, new Vector2(topPipeBounds.X, topPipeBounds.Y), color, new Vector2(topPipeBounds.Width, topPipeBounds.Height));
            BottomPart = new Sprite(texture, new Vector2(bottomPipeBounds.X, bottomPipeBounds.Y), color, new Vector2(bottomPipeBounds.Width, bottomPipeBounds.Height));
            
        }
        public void Update(GameTime gameTime, float speed)
        {
            TopPart.X -= speed;
            BottomPart.X -= speed;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            TopPart.Draw(spriteBatch);
            BottomPart.Draw(spriteBatch);
        }
    }
}
