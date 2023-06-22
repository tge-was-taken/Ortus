using Amicitia.IO;
using Amicitia.IO.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Ortus.Xml
{
    public class XmlDocument
    {
        [StructLayout( LayoutKind.Explicit, Pack = 1, Size = 16 )]
        private unsafe struct BxmBinaryHeader
        {
            [FieldOffset( 0x00 )] public fixed byte Id[ 4 ];
            [FieldOffset( 0x04 )] public uint Unknown;
            [FieldOffset( 0x08 )] public ushort NodeCount;
            [FieldOffset( 0x0A )] public ushort DataCount;
            [FieldOffset( 0x0C )] public uint DataSize;
        }


        [StructLayout( LayoutKind.Explicit, Pack = 1, Size = 8 )]
        private unsafe struct BxmBinaryNode
        {
            [FieldOffset( 0 )] public ushort ChildCount;
            [FieldOffset( 2 )] public ushort FirstChildIndex;
            [FieldOffset( 4 )] public ushort AttributeCount;
            [FieldOffset( 6 )] public ushort DataIndex;
        }

        [StructLayout( LayoutKind.Explicit, Pack = 1, Size = 4 )]
        private unsafe struct BxmBinaryDataEntry
        {
            [FieldOffset( 0 )] public ushort NameOffset;
            [FieldOffset( 2 )] public ushort DataOffset;
        }

        public static Encoding Encoding { get; } =
            CodePagesEncodingProvider.Instance.GetEncoding( 932 );

        public XmlDocumentBinarySignature Signature { get; set; }

        public uint Unknown { get; set; }

        public Endianness Endian { get; set; }

        public Node Root { get; set; }

        private static unsafe bool ValidateSignature( ref BxmBinaryHeader header, string sig ) =>
            header.Id[ 0 ] == sig[ 0 ] && header.Id[ 1 ] == sig[ 1 ] && header.Id[ 2 ] == sig[ 2 ] && header.Id[ 3 ] == sig[ 3 ];

        public static unsafe bool IsValidBinaryFile( string filePath )
        {
            using ( var reader = new BinaryValueReader( filePath,
                FileStreamingMode.Buffered, Endianness.Big, Encoding ) )
            {
                if ( reader.Length < sizeof( BxmBinaryHeader ) )
                    return false;

                var header = reader.Read<BxmBinaryHeader>();
                if ( ( header.DataSize & 0xFF000000 ) != 0 )
                {
                    BinaryOperations<BxmBinaryHeader>.Reverse( ref header );
                    reader.Endianness = Endianness.Little;
                }

                if ( !ValidateSignature( ref header, "BXM\0" ) &&
                     !ValidateSignature( ref header, "XML\0" ) )
                {
                    return false;
                }

                return true;
            }
        }

        public static bool IsValidXmlFile( string filePath )
        {
            using ( var reader = File.OpenText( filePath ) )
            {
                var line = reader.ReadLine();
                if ( line == null ) return false;
                line = line.Trim().ToLower();
                return
                    /* with xml declaration */ line.StartsWith( "<?xml" ) ||
                    /* with leading comment */ ( line.StartsWith( "<!--" ) && line.EndsWith( "-->" ) ) ||
                    /* with leading tag */ ( line.StartsWith( "<" ) && line.EndsWith( ">" ) );
            }
        }

        public static XmlDocument FromBinary( Stream stream )
        {
            using ( var reader = new BinaryValueReader(stream, Amicitia.IO.Streams.StreamOwnership.Transfer, Endianness.Big, Encoding ) )
            {
                XmlDocumentBinarySignature sig;
                var header = reader.Read<BxmBinaryHeader>();
                if ( ( header.DataSize & 0xFF000000 ) != 0 )
                {
                    BinaryOperations<BxmBinaryHeader>.Reverse( ref header );
                    reader.Endianness = Endianness.Little;
                }

                unsafe
                {
                    if ( ValidateSignature( ref header, "XML\0" ) )
                    {
                        sig = XmlDocumentBinarySignature.XML;
                    }
                    else if ( ValidateSignature( ref header, "BXM\0" ) )
                    {
                        sig = XmlDocumentBinarySignature.BXM;
                    }
                    else
                    {
                        throw new InvalidDataException( "Invalid BXM file" );
                    }
                }
                var nodes = reader.ReadArray<BxmBinaryNode>( header.NodeCount );
                var dataTable = reader.ReadArray<BxmBinaryDataEntry>( header.DataCount );
                var baseOffset = reader.Position;
                var nodeCache = new Dictionary<int, Node>();
                var stringCache = new Dictionary<int, string>();

                string ReadString( ushort offset )
                {
                    if ( offset == ushort.MaxValue ) return null;

                    if ( !stringCache.TryGetValue( offset, out var str ) )
                    {
                        reader.Seek( baseOffset + offset, SeekOrigin.Begin );
                        str = reader.ReadString( StringBinaryFormat.NullTerminated );
                        stringCache[ offset ] = str;
                    }

                    return str;
                }

                Node ParseNode( int index )
                {
                    if ( !nodeCache.TryGetValue( index, out var node ) )
                    {
                        ref var binNode = ref nodes[ index ];
                        ref var data = ref dataTable[ binNode.DataIndex ];
                        node = new Node()
                        {
                            Name = ReadString( data.NameOffset ),
                            Value = ReadString( data.DataOffset ),
                        };

                        for ( int i = 0; i < binNode.AttributeCount; i++ )
                        {
                            ref var attrData = ref dataTable[ binNode.DataIndex + 1 + i ];
                            node.Attributes.Add( new Attribute()
                            {
                                Name = ReadString( attrData.NameOffset ),
                                Value = ReadString( attrData.DataOffset ),
                            } );
                        }

                        for ( int i = 0; i < binNode.ChildCount; i++ )
                            node.Children.Add( ParseNode( binNode.FirstChildIndex + i ) );

                        nodeCache[ index ] = node;
                    }

                    return node;
                }

                return new XmlDocument()
                {
                    Root = ParseNode( 0 ),
                    Signature = sig,
                    Unknown = header.Unknown,
                    Endian = reader.Endianness,
                };
            }
        }

        public static XmlDocument FromBinary( string filePath )
        {
            using var stream = File.OpenRead( filePath );
            return FromBinary( stream );
        }

        public static XmlDocument FromXml( string filePath )
        {
            var xDocument = XDocument.Load( filePath );

            Node ConvertElement( XElement xElement )
            {
                var element = new Node
                {
                    Name = xElement.Name.LocalName,
                    Value = xElement.HasElements ? null : xElement.Value,
                };

                foreach ( var attr in xElement.Attributes() )
                {
                    element.Attributes.Add( new Attribute()
                    {
                        Name = attr.Name.LocalName,
                        Value = attr.Value
                    } );
                }

                foreach ( var xChildElement in xElement.Elements() )
                    element.Children.Add( ConvertElement( xChildElement ) );

                return element;
            }

            var xRoot = xDocument.Root;
            var sig = XmlDocumentBinarySignature.BXM;
            var unknown = 0u;
            var endian = Endianness.Big;
            if ( xRoot.Name.LocalName == "__bxm_meta" )
            {
                sig = Enum.Parse<XmlDocumentBinarySignature>( xRoot.Attribute( "signature" )?.Value ?? "BXM" );
                unknown = uint.Parse( xRoot.Attribute( "unknown" )?.Value ?? "0" );
                endian = Enum.Parse<Endianness>( xRoot.Attribute( "endian" )?.Value ?? "Big" );
                xRoot = xRoot.Elements().First();
            }

            foreach ( var node in xDocument.Nodes() )
            {
                if ( node is XComment comment && comment.Value.Trim().StartsWith( "<meta" ) )
                {
                    var metaNode = XElement.Parse( comment.Value.Trim() );
                    sig = Enum.Parse<XmlDocumentBinarySignature>( metaNode.Attribute( "signature" )?.Value ?? "BXM" );
                    unknown = uint.Parse( metaNode.Attribute( "unknown" )?.Value ?? "0" );
                    endian = Enum.Parse<Endianness>( metaNode.Attribute( "endian" )?.Value ?? "Big" );
                }
            }

            return new XmlDocument()
            {
                Root = ConvertElement( xRoot ),
                Signature = sig,
                Unknown = unknown,
                Endian = endian,
            };
        }

        public unsafe void SaveBinary( string filePath )
        {
            using ( var writer = new BinaryObjectWriter( filePath,
                FileStreamingMode.CopyToMemory, Endian, Encoding ) )
            {
                // Convert nodes
                var nodes = new List<BxmBinaryNode>();
                var dataEntries = new List<BxmBinaryDataEntry>();
                var dataIndexLookup = new Dictionary<int, int>();
                var bufferStream = new MemoryStream();
                var bufferWriter = new BinaryValueWriter( bufferStream, Amicitia.IO.Streams.StreamOwnership.Retain, Endian );
                var stringOffsetLookup = new Dictionary<string, int>();
                var nodeQueue = new Queue<Node>();

                ushort WriteString( string str )
                {
                    if ( str == null ) return ushort.MaxValue;

                    if ( !stringOffsetLookup.TryGetValue( str, out var offset ) )
                    {
                        offset = (ushort)bufferWriter.Position;
                        bufferWriter.WriteString( StringBinaryFormat.NullTerminated, str );
                        stringOffsetLookup.Add( str, offset );
                    }

                    return (ushort)offset;
                }

                void ConvertNode( Node node )
                {
                    var localDataEntries = new List<BxmBinaryDataEntry>();

                    var binNode = new BxmBinaryNode()
                    {
                        AttributeCount = (ushort)node.Attributes.Count,
                        ChildCount = (ushort)node.Children.Count,
                        DataIndex = (ushort)dataEntries.Count,
                        FirstChildIndex = (ushort)( nodes.Count + nodeQueue.Count + 1 ),
                    };

                    var binNodeData = new BxmBinaryDataEntry()
                    {
                        NameOffset = WriteString( node.Name ),
                        DataOffset = WriteString( node.Value ),
                    };
                    localDataEntries.Add( binNodeData );

                    var hash = binNodeData.GetHashCode();
                    foreach ( var attr in node.Attributes )
                    {
                        var binAttrData = new BxmBinaryDataEntry()
                        {
                            NameOffset = WriteString( attr.Name ),
                            DataOffset = WriteString( attr.Value ),
                        };
                        localDataEntries.Add( binAttrData );
                        hash = HashCode.Combine( hash, binAttrData.GetHashCode() );
                    }

                    if ( !dataIndexLookup.TryGetValue( hash, out var dataIndex ) )
                    {
                        dataIndex = dataEntries.Count;
                        dataEntries.AddRange( localDataEntries );
                        dataIndexLookup[ hash ] = dataIndex;
                    }

                    binNode.DataIndex = (ushort)dataIndex;
                    nodes.Add( binNode );

                    foreach ( var childNode in node.Children )
                        nodeQueue.Enqueue( childNode );
                }

                nodeQueue.Enqueue( Root );
                while ( nodeQueue.TryDequeue( out var node ) )
                {
                    ConvertNode( node );
                }

                // Write header
                var header = new BxmBinaryHeader();
                switch ( Signature )
                {
                    case XmlDocumentBinarySignature.XML:
                        header.Id[ 0 ] = (byte)'X';
                        header.Id[ 1 ] = (byte)'M';
                        header.Id[ 2 ] = (byte)'L';
                        break;
                    case XmlDocumentBinarySignature.BXM:
                    default:
                        header.Id[ 0 ] = (byte)'B';
                        header.Id[ 1 ] = (byte)'X';
                        header.Id[ 2 ] = (byte)'M';
                        break;
                }

                header.Id[ 3 ] = (byte)0;
                header.Unknown = Unknown;
                header.NodeCount = (ushort)nodes.Count;
                header.DataCount = (ushort)dataEntries.Count;
                header.DataSize = (uint)bufferStream.Length;
                writer.Write( ref header );

                // Write nodes
                writer.WriteCollection( nodes );

                // Write data table
                writer.WriteCollection( dataEntries );

                // Write data buffer
                bufferStream.Position = 0;
                bufferStream.CopyTo( writer.GetBaseStream() );
            }
        }

        public void SaveXml( string filePath )
        {
            var nodeCache = new Dictionary<Node, XElement>();

            XElement ConvertNode( Node node )
            {
                if ( !nodeCache.TryGetValue( node, out var xElement ) )
                {
                    xElement = new XElement( node.Name );
                    if ( node.Value != null )
                        xElement.Value = node.Value;

                    foreach ( var attribute in node.Attributes )
                        xElement.SetAttributeValue( attribute.Name, attribute.Value );

                    foreach ( var childNode in node.Children )
                        xElement.Add( ConvertNode( childNode ) );

                    nodeCache[ node ] = xElement;
                }

                return xElement;
            }

            var xRoot = ConvertNode( Root );

            using var xWriter = XmlWriter.Create( filePath, new XmlWriterSettings()
            {
                Indent = true,
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = false,
            } );

            xWriter.WriteComment( $" <meta signature=\"{Signature}\" unknown=\"{Unknown}\" endian=\"{Endian}\" /> " );
            xRoot.WriteTo( xWriter );
        }
    }
}