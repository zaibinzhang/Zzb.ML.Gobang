using System.Collections.Generic;

namespace Zzb.DeepNeuralNetworks
{
    /// <summary>
    /// 点
    /// </summary>
    public class Node
    {
        public double Value { get; set; }

        public List<Edge> Edges { get; set; }
    }
}