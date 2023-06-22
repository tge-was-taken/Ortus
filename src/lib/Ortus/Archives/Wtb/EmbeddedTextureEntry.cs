using Amicitia.IO.Streams;

namespace Ortus.Archives.Wtb
{
    internal class EmbeddedTextureEntry : ITextureEntry
    {
        internal Stream mBaseStream;

        public uint Flags { get; set; }

        public uint Hash { get; set; }

        public uint Size { get; }

        public uint Offset { get; }

        public EmbeddedTextureEntry( uint flag, uint hash, Stream baseStream, uint offset, uint size )
        {
            Flags        = flag;
            Hash         = hash;
            Size        = size;
            Offset      = offset;
            mBaseStream = baseStream;
        }

        public Stream GetStream()
        {
            if ( mBaseStream == null )
                throw new
                    ExternalTextureDataNotLoadedException( "Attempted to get the data stream of a texture whose contents are stored externally." );

            return new StreamSpan( mBaseStream, Offset, Size );
        }
    }
}