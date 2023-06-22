using Amicitia.IO.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace Ortus.Archives.Dat
{
    [XmlType(TypeName = "Manifest")]
    public class Manifest
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public Endianness Endianness { get; set; }

        [XmlElement]
        public uint MetadataId { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "MetadataIndex")]
        public List<ushort> MetadataIndices { get; set; } = new List<ushort>();

        [XmlArray]
        [XmlArrayItem( ElementName = "File" )]
        public List<FileEntryManifest> Files { get; set; } = new List<FileEntryManifest>();

        public void SaveXmlFile( string path )
        {
            using var manifestFile = File.CreateText( path );
            using var xWriter = XmlWriter.Create( manifestFile, new XmlWriterSettings()
            {
                Indent = true,
            } );

            new XmlSerializer( typeof( Manifest ) ).Serialize( xWriter, this );
        }

        public static Manifest FromXmlFile( string path )
        {
            using var manifestFile = File.OpenText( path );
            return (Manifest)new XmlSerializer( typeof( Manifest ) ).Deserialize( manifestFile );
        }
    }

}