using Amicitia.IO.Streams;

namespace Ortus.Archives.Dat
{
    internal class InternalFileEntry : FileEntry
    {
        private readonly Stream mBaseStream;

        public int Offset { get; }

        public override uint Size { get; }

        public InternalFileEntry( string name, string type, 
            uint hash, ushort id, Stream baseStream, int offset, int size ) 
            : base(name, type, hash, id)
        {
            mBaseStream = baseStream;
            Offset = offset;
            Size = (uint)size;
        }

        public override Stream GetStream()
        {
            return new StreamSpan( mBaseStream, Offset, Size );
        }
    }
}
