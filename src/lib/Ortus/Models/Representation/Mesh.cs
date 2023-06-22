namespace Ortus.Models.Representation
{
    public class Mesh
    {
        public string Name { get; set; }
        public int LodLevel { get; set; }
        public string MaterialName { get; set; }
        public List<Vertex> Vertices { get; set; }
        public List<int> Indices { get; set; }
    }
}
