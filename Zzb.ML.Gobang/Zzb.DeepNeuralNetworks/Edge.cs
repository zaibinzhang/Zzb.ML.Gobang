namespace Zzb.DeepNeuralNetworks
{
    /// <summary>
    /// 边
    /// </summary>
    public class Edge
    {
        public Node FromNode { get; set; }

        public Node ToNode { get; set; }

        public decimal Value { get; set; }
    }
}