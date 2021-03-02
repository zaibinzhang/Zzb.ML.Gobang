using System.Collections.Generic;

namespace Zzb.DeepNeuralNetworks
{
    /// <summary>
    /// 点
    /// </summary>
    public class Node
    {
        public Node()
        {

        }

        public Node(double value)
        {
            Value = value;
        }

        public double Value { get; set; }

        public List<Edge> Edges { get; set; } = new List<Edge>();

        public bool IsNode { get; set; } = true;
    }
}