using Amicitia.IO.Binary;
using Ortus.Models.Utilities;
using System.Collections.Generic;

namespace Ortus.Models.Wmb4
{
    public class Group : IBinarySerializable
    {
        public string Name { get; set; }

        public BoundingBox Extents { get; set; }

        public List<ushort>[] LodMeshIndices { get; }
        public List<ushort> MaterialIndices { get; }

        public Group()
        {
            LodMeshIndices = new List<ushort>[ 4 ];
            for ( var i = 0; i < LodMeshIndices.Length; i++ )
            {
                LodMeshIndices[ i ] = new List<ushort>();
            }
            MaterialIndices = new List<ushort>();
        }

        public Group( string name ) : this()
        {
            Name = name;
        }

        void IBinarySerializable.Read( BinaryObjectReader reader )
        {
            Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Extents = reader.Read<BoundingBox>();

            for ( var index = 0; index < LodMeshIndices.Length; index++ )
            {
                var list = LodMeshIndices[ index ];
                var offset = reader.ReadInt32();
                var count = reader.ReadInt32();

                if ( offset != 0 )
                {
                    reader.ReadAtOffset( offset, reader =>
                    {
                        for ( int i = 0; i < count; i++ )
                            list.Add( reader.ReadUInt16() );
                    } );
                }
            }

            {
                var offset = reader.ReadInt32();
                var count = reader.ReadInt32();

                if ( offset != 0 )
                {
                    reader.ReadAtOffset( offset, reader =>
                    {
                        for ( int i = 0; i < count; i++ )
                            MaterialIndices.Add( reader.ReadUInt16() );
                    } );
                }
            }
        }

        void IBinarySerializable.Write( BinaryObjectWriter writer )
        {
            writer.WriteOffset( () => writer.WriteString( StringBinaryFormat.NullTerminated, Name ), alignment: 16 );
            writer.Write( Extents );

            foreach ( var list in LodMeshIndices )
            {
                if ( list.Count == 0 )
                    writer.Write( 0 );
                else
                    writer.WriteOffset( () => list.ForEach( writer.Write ) );

                writer.Write( list.Count );
            }

            if ( MaterialIndices.Count == 0 )
                writer.Write( 0 );
            else
                writer.WriteOffset( () => MaterialIndices.ForEach( writer.Write ) );

            writer.Write( MaterialIndices.Count );
        }
    }
}