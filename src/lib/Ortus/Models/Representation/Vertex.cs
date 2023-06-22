using System.Numerics;

namespace Ortus.Models.Representation
{
    public class Vertex
    {
        public Vector3 Position { get; set; }
        public Vector2 UV { get; set; }
        public Vector3 Normal { get; set; }
        public Vector3 Tangent { get; set; }
        public List<VertexBoneWeight> BoneWeights { get; set; }
        public Color? Color { get; set; }
        public Vector2? UV2 { get; set; }
        public Color? Color2 { get; set; }
        public Vector2? UV3 { get; set; }
    }
}
