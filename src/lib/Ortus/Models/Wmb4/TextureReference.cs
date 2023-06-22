
using Amicitia.IO.Binary;
using Ortus.Textures;

namespace Ortus.Models.Wmb4
{
    public class TextureReference : IBinarySerializable
    {
        public uint Type { get; set; }
        public uint Hash { get; set; }

        public TextureReference()
        {
        }

        public TextureReference( uint hash ) => Hash = hash;

        public TextureReference( string name )
        {
            Hash = TextureHelper.GetHash( name );
        }

        void IBinarySerializable.Read( BinaryObjectReader reader )
        {
            Type = reader.ReadUInt32();
            Hash = reader.ReadUInt32();
        }

        void IBinarySerializable.Write( BinaryObjectWriter writer )
        {
            writer.Write( Type );
            writer.Write( Hash );
        }
    }
}