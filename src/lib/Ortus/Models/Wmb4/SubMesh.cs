

using Amicitia.IO.Binary;

namespace Ortus.Models.Wmb4
{
    public class SubMesh : IBinarySerializable
    {
        public int BufferGroupIndex { get; set; }
        public int VertexBufferStartIndex { get; set; }
        public int IndexBufferStartIndex { get; set; }
        public int VertexCount { get; set; }
        public int IndexCount { get; set; }

        public SubMesh()
        {
        }

        public SubMesh( int bufferGroupIndex, int vertexBufferStartIndex, int indexBufferStartIndex, int vertexCount, int triangleCount )
        {
            BufferGroupIndex = bufferGroupIndex;
            VertexBufferStartIndex = vertexBufferStartIndex;
            IndexBufferStartIndex = indexBufferStartIndex;
            VertexCount = vertexCount;
            IndexCount = triangleCount;
        }

        void IBinarySerializable.Read( BinaryObjectReader reader )
        {
            BufferGroupIndex = reader.ReadInt32();
            VertexBufferStartIndex = reader.ReadInt32();
            IndexBufferStartIndex = reader.ReadInt32();
            VertexCount = reader.ReadInt32();
            IndexCount = reader.ReadInt32();
        }

        void IBinarySerializable.Write( BinaryObjectWriter writer )
        {
            writer.Write( BufferGroupIndex );
            writer.Write( VertexBufferStartIndex );
            writer.Write( IndexBufferStartIndex );
            writer.Write( VertexCount );
            writer.Write( IndexCount );
        }
    }
}
