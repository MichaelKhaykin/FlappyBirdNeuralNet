using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlappyBirdNeuralNet
{
    public class Layer
    {
        public List<Neuron> Neurons;

        public int NeuronCount => Neurons.Count;

        public Layer()
        {
            Neurons = new List<Neuron>();
        }
    }
}
