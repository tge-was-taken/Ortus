namespace Ortus.Xml
{
    public class Node
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public IList<Node> Children { get; }
        public IList<Attribute> Attributes { get; }

        public Node()
        {
            Children = new List<Node>();
            Attributes = new List<Attribute>();
        }

        public override string ToString()
        {
            return $"{Name} = {Value} ({Children.Count} children, {Attributes.Count} attributes)";
        }
    }
}