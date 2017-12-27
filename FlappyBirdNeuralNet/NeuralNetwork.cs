using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FlappyBirdNeuralNet
{
    public class NeuralNetwork
    {
        [JsonProperty]
        List<Layer> Layers;

        [JsonProperty]
        int[] layersAmount;

        [JsonConstructor]
        public NeuralNetwork(int[] layersAmount)
        {
            if (layersAmount.Length < 2)
            {
                return;
            }

            this.layersAmount = layersAmount;

            Layers = new List<Layer>();
            for (int i = 0; i < layersAmount.Length; i++)
            {
                var layer = new Layer();
                Layers.Add(layer);
                for (int j = 0; j < layersAmount[i]; j++)
                {
                    layer.Neurons.Add(new Neuron());
                }
                if (i == 0)
                {
                    for (int k = 0; k < layer.Neurons.Count; k++)
                    {
                        layer.Neurons[k].Bias = 0;
                    }
                }
                else
                {
                    for (int l = 0; l < layer.Neurons.Count; l++)
                    {
                        for (int n = 0; n < layersAmount[i - 1]; n++)
                        {
                            layer.Neurons[l].Dendrites.Add(new Dendrite());
                        }
                    }
                }
            }
        }

        public float Sigmoid(float x)
        {
            return (float)(1 / (1 + Math.Exp(-x)));
        }

        public float[] Run(List<float> inputs)
        {
            if (inputs.Count != Layers[0].NeuronCount)
            {
                return null;
            }

            for (int i = 0; i < Layers.Count; i++)
            {
                var layer = Layers[i];
                for (int j = 0; j < layer.NeuronCount; j++)
                {
                    var neuron = layer.Neurons[j];
                    if (i == 0)
                    {
                        neuron.Value = inputs[j];
                    }
                    else
                    {
                        neuron.Value = 0;
                        for (int z = 0; z < neuron.DendriteCount; z++)
                        {
                            neuron.Value += Layers[i - 1].Neurons[z].Value * Layers[i].Neurons[j].Dendrites[z].Weight;
                        }
                        neuron.Value = Sigmoid(neuron.Value + neuron.Bias);
                    }
                }

            }
            Layer lastLayer = Layers[Layers.Count - 1];
            float[] output = new float[lastLayer.NeuronCount];

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = lastLayer.Neurons[i].Value;
            }

            return output;
        }

        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < Layers.Count; i++)
            {
                //var layer = Layers[i];
                for (int j = 0; j < Layers[i].NeuronCount; j++)
                {
                    //var neuron = layer.Neurons[j];

                    //mutate bias here
                    Random random = new Random(Guid.NewGuid().GetHashCode());
                    double doesChange = random.NextDouble();
                    if (doesChange < mutationRate)
                    {
                        Layers[i].Neurons[j].Bias += (float)(random.Next(-1, 1) + random.NextDouble());
                    }

                    for (int k = 0; k < Layers[i].Neurons[j].DendriteCount; k++)
                    {
                        doesChange = random.NextDouble();
                        if (doesChange >= mutationRate) continue;

                        //var dendDrite = neuron.Dendrites[i];
                        Layers[i].Neurons[j].Dendrites[k].Weight += (float)(random.Next(-1, 1) + random.NextDouble());
                    }
                }
            }
        }

        public void Clone(NeuralNetwork nn) //clone
        {
            if (Layers.Count != nn.Layers.Count) return;
            for (int i = 0; i < Layers.Count; i++)
            {
                if (Layers[i].NeuronCount != nn.Layers[i].NeuronCount) return;
                for (int j = 0; j < Layers[i].NeuronCount; j++)
                {
                    if (Layers[i].Neurons[j].DendriteCount != nn.Layers[i].Neurons[j].DendriteCount) return;
                    Layers[i].Neurons[j].Bias = nn.Layers[i].Neurons[j].Bias;
                    for (int k = 0; k < Layers[i].Neurons[j].DendriteCount; k++)
                    {
                        Layers[i].Neurons[j].Dendrites[k].Weight = nn.Layers[i].Neurons[j].Dendrites[k].Weight;
                    }
                }
            }
            //loop through all dentrites and set them equal to the nn dentrites
        }
    }
}