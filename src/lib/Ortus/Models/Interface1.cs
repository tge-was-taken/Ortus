using Amicitia.IO.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ortus.Models
{
    public class AssetReader : BinaryObjectReader
    {
        public AssetReader( string filePath, Endianness endianness, Encoding encoding ) : base( filePath, endianness, encoding )
        {
        }
    }

    public class AssetWriter : BinaryObjectWriter
    {
        public AssetWriter( string filePath, Endianness endianness, Encoding encoding ) : base( filePath, endianness, encoding )
        {
        }
    }

    [AttributeUsage( AttributeTargets.All, Inherited = false, AllowMultiple = false )]
    public sealed class AssetDefinitionAttribute : Attribute
    {
        private string signature;
        private string description;
        private string[] extensions;

        public AssetDefinitionAttribute( string signature, string description, string[] extensions )
        {
            this.signature = signature;
            this.description = description;
            this.extensions = extensions;
        }
    }

    public interface IAsset : IBinarySerializable
    {
        IAsset Parent { get; }
        string Name { get; set; }
        string Extension { get; set; }

        void Load( AssetReader reader );
        void Save( AssetWriter writer );
    }

    internal interface IAssetBundle
    {
        IList<IAsset> Assets { get; }
    }
}
