using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlappyBirdNeuralNet
{
    public class Neuron
    {
        public float Value;
        public List<Dendrite> Dendrites;
        public float Bias;
        public int DendriteCount => Dendrites.Count;

        public Neuron()
        {
            Random gen = new Random(Guid.NewGuid().GetHashCode());
            Bias = (float)gen.NextDouble();
            Value = 0;
            Dendrites = new List<Dendrite>();
        }
    }
}
