

using Amicitia.IO.Binary;

namespace Ortus.Models.Wmb4
{
    public class MatrixPalette : IBinarySerializable
    {
        public byte[] MatrixToNodeIndices { get; set; }

        public MatrixPalette()
        {
            MatrixToNodeIndices = Array.Empty<byte>();
        }

        void IBinarySerializable.Read( BinaryObjectReader reader )
        {
            var boneIndexListOffset = reader.ReadInt32();
            var boneIndexCount = reader.ReadInt32();

            if ( boneIndexListOffset != 0 )
                reader.ReadAtOffset( boneIndexListOffset, () => MatrixToNodeIndices = reader.ReadArray<byte>( boneIndexCount ) );
        }

        void IBinarySerializable.Write( BinaryObjectWriter writer )
        {
            if ( MatrixToNodeIndices == null || MatrixToNodeIndices.Length == 0 )
                writer.Write( 0 );
            else
                writer.WriteOffset( () => writer.WriteArray( MatrixToNodeIndices ), alignment: 16 );

            writer.Write( MatrixToNodeIndices?.Length ?? 0 );
        }
    }
}
