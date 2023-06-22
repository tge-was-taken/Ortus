using Amicitia.IO.Binary;
using Amicitia.IO.Binary.Extensions;
using Amicitia.IO.Streams;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Force.Crc32;
using Ortus.Utilities;

namespace Ortus.Archives.Dat
{
    public class DatArchive : IDisposable
    {
        private const string MAGIC = "DAT\0";
        private const int FILE_ALIGNMENT = 0x1000;

        private Stream mStream;

        public static Encoding Encoding = 
            CodePagesEncodingProvider.Instance.GetEncoding( 932 );

        public List<FileEntry> Files { get; } = new List<FileEntry>();

        public Endianness Endianness { get; set; }

        public uint MetadataId { get; set; }

        public List<ushort> MetadataIndices { get; } = new List<ushort>();

        public FileEntry this[ string name ] => Files.Single( x => x.Name == name );

        public DatArchive()
        {
            MetadataId = 0x1E;
        }

        public DatArchive( string filepath ) : this( File.OpenRead( filepath ) ) { }

        public DatArchive( Stream stream )
        {
            mStream = stream;

            using ( var reader = new BinaryObjectReader( stream, StreamOwnership.Retain, Endianness.Little, Encoding ) )
                Read( reader );
        }

        public void Save( Stream stream, bool leaveOpen )
        {
            using ( var writer = new BinaryObjectWriter( stream, leaveOpen ? StreamOwnership.Retain : StreamOwnership.Transfer, Endianness, Encoding ) )
            {
                Write( writer );
                writer.Flush();
            }
        }

        public void Save( string filepath ) => Save( File.Create( filepath ), false );

        public MemoryStream Save()
        {
            var stream = new MemoryStream();
            Save( stream, true );
            stream.Position = 0;
            return stream;
        }

        public Stream OpenFile( string name )
        {
            var res = this[ name ];
            return res.GetStream();
        }

        public void AddFile( string filepath )
        {
            AddFile( Path.GetFileName( filepath ), File.OpenRead( filepath ) );
        }

        public void AddFile( string name, Stream stream )
        {
            MetadataIndices.Add( (ushort)Files.Count );
            MetadataIndices.Add( ushort.MaxValue );
            Files.Add( new ExternalFileEntry( name, Path.GetExtension( name ).Substring( 1 ),
                Hasher.ComputeNameHash( name ), 0, stream ) );
        }

        public void ReplaceFile( string name, Stream stream )
        {
            var resIndex = Files.FindIndex( x => x.Name == name );
            var res = Files[ resIndex ];
            Files[ resIndex ] = new ExternalFileEntry( res.Name, res.Type, res.Hash, res.Id, stream );
        }

        public void ReplaceFile( string name, string filePath ) => ReplaceFile( name, File.OpenRead( filePath ) );

        internal void Read( BinaryObjectReader reader )
        {
            var magic = Encoding.GetString( reader.ReadArray<byte>( 4 ) );
            if ( magic != MAGIC )
                throw new InvalidFileFormatException( "The header magic value does not match the expected value." );

            var resourceCount = reader.ReadInt32();
            if ((resourceCount & 0xFF000000) != 0)
            {
                Endianness = Endianness.Big;
                reader.Endianness = Endianness;
                BinaryOperations<int>.Reverse( ref resourceCount );
            }

            var resourceOffsets = new int[resourceCount];
            var resourceTypes = new string[resourceCount];
            var resourceNames = new string[resourceCount];
            var resourceSizes = new int[resourceCount];
            var resourceHashes = new uint[resourceCount];
            var resourceIds = new ushort[resourceCount];

            reader.ReadArrayOffset( resourceCount, (reader, i) => resourceOffsets[i] = reader.ReadInt32() );
            reader.ReadArrayOffset( resourceCount, (reader, i) => resourceTypes[ i ] = reader.ReadString( StringBinaryFormat.NullTerminated ) );
            reader.ReadOffset( () =>
            {
                var resourceMaxNameLength = reader.ReadInt32();
                for ( int i = 0; i < resourceCount; i++ )
                    resourceNames[ i ] = reader.ReadString( StringBinaryFormat.FixedLength, resourceMaxNameLength );
            } );
            reader.ReadArrayOffset( resourceCount, (reader, i) => resourceSizes[ i ] = reader.ReadInt32() );
            reader.ReadOffset( () =>
            {
                reader.PushOffsetOrigin();

                MetadataId = reader.ReadUInt32();
                var metadataIndicesListOffset = reader.ReadInt32();
                var resourceHashListOffset    = reader.ReadInt32();
                var resourceIdListOffset      = reader.ReadInt32();

                reader.ReadAtOffset( metadataIndicesListOffset, () =>
                {
                    var endOffset = reader.Position + ( resourceHashListOffset - metadataIndicesListOffset );
                    while ( reader.Position < endOffset )
                    {
                        MetadataIndices.Add( reader.ReadUInt16() );
                    }
                } );

                reader.ReadAtOffset( resourceHashListOffset, () =>
                {
                    for ( int i = 0; i < resourceHashes.Length; i++ )
                        resourceHashes[ i ] = reader.ReadUInt32();
                } );

                reader.ReadAtOffset( resourceIdListOffset, () =>
                {
                    for ( int i = 0; i < resourceIds.Length; i++ )
                        resourceIds[ i ] = reader.ReadUInt16();
                } );

                reader.PopOffsetOrigin();
            } );

            Files.Capacity = resourceCount;
            for ( int i = 0; i < resourceCount; i++ )
            {
                var resource = new InternalFileEntry( resourceNames[ i ], resourceTypes[ i ], resourceHashes[i], resourceIds[i], mStream,
                                                      resourceOffsets[ i ], resourceSizes[ i ] );

                Files.Add( resource );
            }
        }

        internal void Write( BinaryObjectWriter writer )
        {
            writer.WriteString( StringBinaryFormat.FixedLength, MAGIC, 4 );
            writer.Write( Files.Count );
            writer.WriteOffset( reader =>
            {
                foreach ( var file in Files )
                {
                    writer.WriteOffset( reader =>
                    {
                        file.GetStream().CopyTo( writer.BaseStream );
                    }, alignment: FILE_ALIGNMENT );
                }
            } );

            writer.WriteOffset( () => Files.ForEach( x => writer.WriteString( StringBinaryFormat.FixedLength, x.Type, 4 ) ) );
            writer.WriteOffset( () =>
            {
                var resourceMaxNameLength = Files.Max( x => x.Name.Length ) + 1;
                writer.Write( resourceMaxNameLength );
                Files.ForEach( x => writer.WriteString( StringBinaryFormat.FixedLength, x.Name, resourceMaxNameLength ) );
            } );
            writer.WriteOffset( () => Files.ForEach( x => writer.Write( x.Size ) ), alignment: 4 );
            writer.WriteOffset( () =>
            {
                writer.PushOffsetOrigin();

                writer.Write(MetadataId);
                writer.WriteOffset( () => 
                MetadataIndices.ForEach( writer.Write ) );
                writer.WriteOffset( () => Files.ForEach( x => writer.Write( x.Hash ) ) );
                writer.WriteOffset( () => Files.ForEach( x => writer.Write( x.Id ) ) );

                writer.PopOffsetOrigin();
            });
            writer.Align( 16 );
        }

        public void Dispose()
        {
            ( (IDisposable)mStream ).Dispose();
        }
    }
}
