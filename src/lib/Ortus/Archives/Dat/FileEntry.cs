namespace Ortus.Archives.Dat
{
    public abstract class FileEntry : IEntry
    {
        private string mName;

        public string Name
        {
            get => mName;
            set
            {
                Type = Path.GetExtension( value ).Substring( 1 );
                mName = value;
            }
        }

        public string Type { get; set; }

        public abstract uint Size { get; }

        public uint Hash { get; set; }

        public ushort Id { get; }

        protected FileEntry( string name, string type, uint hash, ushort id )
        {
            mName = name;
            Type = type;
            Hash = hash;
            Id = id;
        }

        public abstract Stream GetStream();
    }
}
