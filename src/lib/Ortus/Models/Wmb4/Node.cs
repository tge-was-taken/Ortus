using Amicitia.IO.Binary;
using System.Numerics;


namespace Ortus.Models.Wmb4
{
    public class Bone : IBinarySerializable
    {
        public ushort Id { get; set; }

        public ushort Flags { get; set; }

        public ushort ParentIndex { get; set; }

        public ushort Field08 { get; set; }

        public Vector3 Local { get; set; }

        public Vector3 World { get; set; }

        void IBinarySerializable.Read( BinaryObjectReader reader )
        {
            Id = reader.ReadUInt16();
            Flags = reader.ReadUInt16();
            ParentIndex = reader.ReadUInt16();
            Field08 = reader.ReadUInt16();
            Local = reader.Read<Vector3>();
            World = reader.Read<Vector3>();
        }

        void IBinarySerializable.Write( BinaryObjectWriter writer )
        {
            writer.Write( Id );
            writer.Write( Flags );
            writer.Write( ParentIndex );
            writer.Write( Field08 );
            writer.Write( Local );
            writer.Write( World );
        }
    }
}
