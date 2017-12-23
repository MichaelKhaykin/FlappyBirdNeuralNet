using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlappyBirdNeuralNet
{
    public class Dendrite
    {
        public float Weight;

        public Dendrite()
        {
            Random gen = new Random(Guid.NewGuid().GetHashCode());
            Weight = (float)gen.NextDouble();
        }
    }
}
