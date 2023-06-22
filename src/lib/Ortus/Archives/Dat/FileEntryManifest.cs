using System.Xml;
using System.Xml.Serialization;

namespace Ortus.Archives.Dat
{
    public class FileEntryManifest
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public uint Hash { get; set; }

        [XmlAttribute]
        public ushort Id { get; set; }
    }

}