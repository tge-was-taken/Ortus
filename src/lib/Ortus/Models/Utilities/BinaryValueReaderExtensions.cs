using Amicitia.IO.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ortus.Models.Utilities
{
    internal static class BinaryValueReaderExtensions
    {
        public static Vector2 ReadVector2Half( this BinaryValueReader reader )
        {
            return new Vector2( (float)reader.Read<Half>(), (float)reader.Read<Half>() );
        }

        public static Vector3 ReadVector3Packed( this BinaryValueReader reader )
        {
            return VectorCompressor.Decompress_11_11_10( reader.Read<uint>() );
        }
    }

    internal static class BinaryValueWriterExtensions
    {
        public static void WriteVector2Half( this BinaryValueWriter writer, Vector2 value )
        {
            writer.Write( (Half)value.X );
            writer.Write( (Half)value.Y );
        }

        public static void WriteVector3Packed( this BinaryValueWriter writer, Vector3 value )
        {
            writer.Write( VectorCompressor.Compress_11_11_10( value ) );
        }
    }
}
