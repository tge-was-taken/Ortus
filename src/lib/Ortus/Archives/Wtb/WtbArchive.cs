using Amicitia.IO.Binary;
using Amicitia.IO.Binary.Extensions;
using Amicitia.IO.Streams;
using Ortus.Archives;
using Ortus.Archives.Dat;
using Ortus.Utilities;
using System.Text;

namespace Ortus.Archives.Wtb
{
    public class WtbArchive
    {
        private const int MAGIC = 0x00425457;

        private DatArchive mTextureArchive;
        private Stream mTextureStream;

        public DataStorageMode StorageMode { get; set; }

        public int Field04 { get; set; }

        public List<ITextureEntry> Textures { get; } = new List<ITextureEntry>();

        public WtbArchive() { }

        public WtbArchive( DataStorageMode mode ) => StorageMode = mode;

        public WtbArchive( string filepath ) : this( File.OpenRead( filepath ) ) { }

        public WtbArchive( string filepath, string externalTextureBankFilePath )
        {
            LoadExternalDataStorage( externalTextureBankFilePath );

            using ( var reader = new BinaryObjectReader( File.OpenRead( filepath ), StreamOwnership.Transfer, Endianness.Little, Encoding.Default ) )
                Read( reader );
        }

        /// <summary>
        /// Read texture bank from a stream. Stream will remain in use if the textures are not stored externally, otherwise the stream (if allowed) will be disposed.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="leaveOpen"></param>
        public WtbArchive( Stream stream, bool leaveOpen = true )
        {
            using ( var reader = new BinaryObjectReader( stream, StreamOwnership.Retain, Endianness.Little, Encoding.Default ) )
                Read( reader );

            if ( StorageMode == DataStorageMode.External && !leaveOpen )
                stream.Dispose();
        }

        public void Save( Stream stream, bool leaveOpen = true, string externalTexturePackName = null, Stream externalTexturePackStream = null, bool leaveOpen2 = true )
        {
            if ( StorageMode == DataStorageMode.External && externalTexturePackStream == null )
                throw new ArgumentException( $"External texture pack stream can't be null if external storage is used.",
                                             nameof( externalTexturePackStream ) );

            using ( var writer = new BinaryObjectWriter( stream, StreamOwnership.Transfer, Endianness.Little, Encoding.Default ) )
            {
                Write( writer, externalTexturePackStream, Path.ChangeExtension( externalTexturePackName, "wtp" ) );
            }

            if ( externalTexturePackStream != null && !leaveOpen2 )
                externalTexturePackStream.Dispose();
        }

        public void Save( Stream stream, bool leaveOpen = true )
        {
            Save( stream, leaveOpen, null );
        }

        public void Save( string filepath ) => Save( File.Create( filepath ), false );

        public void Save( string filepath, string externalTextureBankFilePath )
        {
            Save( FileHelper.Create( filepath ),                    false, Path.GetFileNameWithoutExtension( externalTextureBankFilePath ),
                  FileHelper.Create( externalTextureBankFilePath ), false );
        }

        public MemoryStream Save()
        {
            var stream = new MemoryStream();
            Save( stream, true );
            return stream;
        }

        public void LoadExternalDataStorage( string path )
        {
            LoadExternalDataStorage( File.OpenRead( path ), Path.GetExtension( path ) );
        }

        public void LoadExternalDataStorage( Stream stream, string formatHint )
        {
            if ( StorageMode == DataStorageMode.Embedded )
                throw new
                    InvalidOperationException( "Tried to load external texture pack for a texture bank whose textures are not stored externally" );

            if ( formatHint == ".dtt" )
            {
                mTextureArchive = new DatArchive( stream );
                var wtpFile = mTextureArchive.Files.FirstOrDefault( x => x.Type == "wtp" );
                if ( wtpFile == null )
                    throw new InvalidOperationException( "Tried to load external texture pack from an archive that does not contain one" );

                mTextureStream = mTextureArchive.Files[ 0 ].GetStream();
            }
            else
            {
                mTextureStream = stream;
            }

            foreach ( var texture in Textures )
            {
                if ( texture is EmbeddedTextureEntry embeddedTexture )
                    embeddedTexture.mBaseStream = mTextureStream;
            }
        }

        internal void Read( BinaryObjectReader reader )
        {
            var magic = reader.ReadInt32();
            if ( magic != MAGIC )
                throw new InvalidFileFormatException( "The header magic value does not match the expected value." );

            Field04 = reader.ReadInt32();

            var textureCount = reader.ReadInt32();
            uint[] textureOffsets = null, textureSizes = null, textureFlags = null, textureIds = null;

            reader.ReadOffset( () => textureOffsets = reader.ReadArray<uint>( textureCount ) );
            reader.ReadOffset( () => textureSizes = reader.ReadArray<uint>( textureCount ) );
            reader.ReadOffset( () => textureFlags = reader.ReadArray<uint>( textureCount ) );
            reader.ReadOffset( () => textureIds = reader.ReadArray<uint>( textureCount ) );

            StorageMode = textureOffsets.Any( x => x == 0 || x >= reader.BaseStream.Length ) ? DataStorageMode.External : DataStorageMode.Embedded;

            for ( int i = 0; i < textureCount; i++ )
            {
                Textures.Add( new EmbeddedTextureEntry( 
                    textureFlags[ i ], 
                    textureIds?[ i ] ?? 0, 
                    StorageMode == DataStorageMode.External ? mTextureStream : reader.BaseStream,
                    textureOffsets[ i ], 
                    textureSizes[ i ] ) );
            }
        }

        internal void Write( BinaryObjectWriter writer, Stream externalTextureStream, string name )
        {
            var tempTextureStream = new MemoryStream();

            writer.Write( MAGIC );
            writer.Write( Field04 );
            writer.Write( Textures.Count );
            writer.WriteOffset( () =>
            {
                if ( StorageMode == DataStorageMode.External )
                {
                    using ( var textureWriter = new BinaryObjectWriter( tempTextureStream, StreamOwnership.Retain, Endianness.Little ) )
                    {
                        Textures.ForEach( x =>
                        {
                            writer.Write( ( int )textureWriter.Position );
                            using ( var textureStream = x.GetStream() )
                                textureStream.CopyTo( textureWriter.BaseStream );

                            textureWriter.Align( 2048 );
                        } );
                    }
                }
                else
                {
                    Textures.ForEach( x =>
                    {
                        writer.WriteOffset( () =>
                        {
                            using ( var textureStream = x.GetStream() )
                                textureStream.CopyTo( writer.BaseStream );

                            writer.Align( 2048 );
                        } );
                    } );
                }
            }, alignment: 32 );
            writer.WriteOffset( () => Textures.ForEach( x => writer.Write( x.Size ) ), alignment: 32 );
            writer.WriteOffset( () => Textures.ForEach( x => writer.Write( x.Flags ) ), alignment: 32 );
            if ( Field04 != 0 )
            {
                writer.WriteOffset( () =>
                {
                    Textures.ForEach( x => writer.Write( x.Hash ) );
                    writer.Align( 32 );
                }, alignment: 32 );
            }
            writer.Flush();

            if ( externalTextureStream != null )
            {
                var archive = new DatArchive();
                archive.AddFile( name, tempTextureStream );
                archive.Save( externalTextureStream, true );
            }
        }

        public void Dispose()
        {
            mTextureStream.Dispose();
        }
    }

    public enum DataStorageMode
    {
        Embedded,
        External
    }
}
