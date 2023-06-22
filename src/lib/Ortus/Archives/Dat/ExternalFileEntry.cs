using Amicitia.IO.Streams;

namespace Ortus.Archives.Dat
{
    public class ExternalFileEntry : FileEntry
    {
        private readonly Stream mStream;

        public override uint Size => ( uint ) mStream.Length;

        public ExternalFileEntry( string name, string type, uint hash, ushort id, Stream stream ) : base(name, type, hash, id)
        {
            mStream = stream;
        }

        public override Stream GetStream()
        {
            return new StreamSpan( mStream, 0, mStream.Length );
        }
    }   
}
