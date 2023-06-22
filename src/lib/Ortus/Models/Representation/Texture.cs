namespace Ortus.Models.Representation
{
    public class Texture
    {
        public string Name { get; set; }
        public uint Type { get; set; } = 0;
        public uint Hash { get; set; }
        public uint Flags { get; set; } = 0x20000020;
    }
}
