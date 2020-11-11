using System.Collections.Generic;

namespace Zzb.DeepNeuralNetworks
{
    public class NetworkLayer
    {
        private List<double> _neurons = new List<double>();

        private List<List<double>> _ws = new List<List<double>>();

        private NetworkLayer _previousLayer;

        private NetworkLayer _nextLayer;

        private double _b = 1;

        private void ForwardPropagation()
        {
            if (_nextLayer == null)
            {
                return;
            }


        }
    }
}