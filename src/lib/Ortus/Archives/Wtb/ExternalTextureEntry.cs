using Amicitia.IO.Streams;
using Ortus.Textures;
using Ortus.Textures.Utilities;

namespace Ortus.Archives.Wtb
{
    public class ExternalTextureEntry : ITextureEntry
    {
        private const uint FLAG = 0x20000020;

        private readonly Stream mStream;

        public uint Flags { get; set; }

        public uint Hash { get; set; }

        public uint Size { get; }

        public ExternalTextureEntry( uint flag, uint hash, Stream stream )
        {
            Flags    = flag;
            Hash     = hash;
            Size    = (uint) stream.Length;
            mStream = stream;
        }

        public ExternalTextureEntry( uint hash, Stream stream )
        {
            Flags    = FLAG;
            Hash     = hash;
            Size    = (uint)stream.Length;
            mStream = stream;
        }

        public ExternalTextureEntry( uint hash, byte[] data ) : this( hash, new MemoryStream( data ) ) { }

        public ExternalTextureEntry( string path )
        {
            Flags    = FLAG;
            Hash     = TextureHelper.GetHash( path );
            mStream = DDSHelper.GetDDSStream( path );
            Size    = (uint) mStream.Length;
        }

        public Stream GetStream()
        {
            return new StreamSpan( mStream, 0, mStream.Length );
        }
    }
}