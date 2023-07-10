using System.Numerics;

namespace Ortus.Models.Wmb4
{
    public class Vertex : IEquatable<Vertex>, IComparable<Vertex>
    {
        public Vector3? Position { get; set; }

        public Vector2? UV { get; set; }

        public Vector3? Normal { get; set; }

        public Vector3? Tangent { get; set; }

        public byte[] BoneIndices { get; set; }

        public float[] BoneWeights { get; set; }

        public Vector2? UV2 { get; set; }

        public uint? Color { get; set; }

        public Vertex()
        {
            BoneIndices = new byte[4];
            BoneWeights = new float[4];
        }

        public int CompareTo( Vertex? other )
        {
            if ( other == this ) return 0;
            return -1;
        }

        public bool Equals( Vertex? other )
        {
            if ( other == null ) return false;
            if ( BoneIndices == null && other.BoneIndices != null ) return false;
            if ( BoneWeights == null && other.BoneWeights != null ) return false;
            if ( BoneIndices != null && !BoneIndices.SequenceEqual( other.BoneIndices ) ) return false;
            if ( BoneWeights != null && !BoneWeights.SequenceEqual( other.BoneWeights ) ) return false;
            return Position == other.Position && UV == other.UV && Normal == other.Normal &&
                Tangent == other.Tangent && UV2 == other.UV2 && Color == other.Color;
        }
    }
}