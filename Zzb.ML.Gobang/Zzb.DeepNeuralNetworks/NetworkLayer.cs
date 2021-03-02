using System.Collections.Generic;

namespace Zzb.DeepNeuralNetworks
{
    /// <summary>
    /// 神经网络层
    /// </summary>
    public class NetworkLayer
    {
        public List<Node> Nodes { get; set; }

        public NetworkLayer PreLayer { get; set; }

        public NetworkLayer NextLayer { get; set; }
    }
}