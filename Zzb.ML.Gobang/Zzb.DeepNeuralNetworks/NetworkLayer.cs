using System.Collections.Generic;

namespace Zzb.DeepNeuralNetworks
{
    public class NetworkLayer
    {
        private List<Neuron> _neurons = new List<Neuron>();

        private NetworkLayer _forwardLayer;

        private NetworkLayer _nextLayer;
    }
}