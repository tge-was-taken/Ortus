

using Amicitia.IO.Binary;

namespace Ortus.Models.Wmb4
{
    public class Mesh : IBinarySerializable
    {
        public int SubMeshIndex { get; set; }
        public int GroupIndex { get; set; }
        public short MaterialIndex { get; set; }
        public short MatrixPaletteIndex { get; set; }
        public int Field0C { get; set; }

        public Mesh()
        {
            Field0C = 0x100;
        }

        public Mesh( int subMeshIndex, int groupIndex, short materialIndex, short matrixPaletteIndex )
        {
            SubMeshIndex = subMeshIndex;
            GroupIndex = groupIndex;
            MaterialIndex = materialIndex;
            MatrixPaletteIndex = matrixPaletteIndex;
            Field0C = 0x100;
        }

        void IBinarySerializable.Read( BinaryObjectReader reader )
        {
            SubMeshIndex = reader.ReadInt32();
            GroupIndex = reader.ReadInt32();
            MaterialIndex = reader.ReadInt16();
            MatrixPaletteIndex = reader.ReadInt16();
            Field0C = reader.ReadInt32();
        }

        void IBinarySerializable.Write( BinaryObjectWriter writer )
        {
            writer.Write( SubMeshIndex );
            writer.Write( GroupIndex );
            writer.Write( MaterialIndex );
            writer.Write( MatrixPaletteIndex );
            writer.Write( Field0C );
        }
    }
}
