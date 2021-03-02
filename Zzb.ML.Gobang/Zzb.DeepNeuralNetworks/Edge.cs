namespace Zzb.DeepNeuralNetworks
{
    /// <summary>
    /// 边
    /// </summary>
    public class Edge
    {
        public Edge()
        {

        }

        public Edge(Node fromNode, Node toNode, double value)
        {
            FromNode = fromNode;
            ToNode = toNode;
            Value = value;
        }

        public void Connect()
        {
            FromNode.Edges.Add(this);
            ToNode.Edges.Add(this);
        }

        public Node FromNode { get; set; }

        public Node ToNode { get; set; }

        public double Value { get; set; }
    }
}