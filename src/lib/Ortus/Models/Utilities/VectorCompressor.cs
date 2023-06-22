using System;
using System.Numerics;

namespace Ortus.Models.Utilities
{
    public static class VectorCompressor
    {
        public static Vector3 Decompress_11_11_10( uint compressed )
        {
            const int FULL_WIDTH = 32;

            const int X_POS    = 0;
            const int X_WIDTH  = 11;
            const int X_LSHIFT = FULL_WIDTH - X_POS - X_WIDTH;
            const int X_RSHIFT = FULL_WIDTH - X_WIDTH;
            const int X_DOT    = ( 1 << ( X_WIDTH - 1 ) ) - 1;

            const int Y_POS    = X_POS + X_WIDTH;
            const int Y_WIDTH  = 11;
            const int Y_LSHIFT = FULL_WIDTH - Y_POS - Y_WIDTH;
            const int Y_RSHIFT = FULL_WIDTH - Y_WIDTH;
            const int Y_DOT    = ( 1 << ( Y_WIDTH - 1 ) ) - 1;

            const int Z_POS    = Y_POS + Y_WIDTH;
            const int Z_WIDTH  = 10;
            const int Z_LSHIFT = FULL_WIDTH - Z_POS - Z_WIDTH;
            const int Z_RSHIFT = FULL_WIDTH - Z_WIDTH;
            const int Z_DOT    = ( 1 << ( Z_WIDTH - 1 ) ) - 1;

            int xInt = ( ( int )( compressed << X_LSHIFT ) ) >> X_RSHIFT;
            int yInt = ( ( int )( compressed << Y_LSHIFT ) ) >> Y_RSHIFT;
            int zInt = ( ( int )( compressed << Z_LSHIFT ) ) >> Z_RSHIFT;

            Vector3 value;
            value.X = ( float )xInt / ( float )X_DOT;
            value.Y = ( float )yInt / ( float )Y_DOT;
            value.Z = ( float )zInt / ( float )Z_DOT;

            return value;
        }

        public static uint Compress_11_11_10( Vector3 uncompressed )
        {
            const int X_POS = 0;
            const int X_WIDTH = 11;
            const int X_DOT = ( 1 << ( X_WIDTH - 1 ) ) - 1;

            const int Y_POS = X_POS + X_WIDTH;
            const int Y_WIDTH = 11;
            const int Y_DOT = ( 1 << ( Y_WIDTH - 1 ) ) - 1;

            const int Z_POS = Y_POS + Y_WIDTH;
            const int Z_WIDTH = 10;
            const int Z_DOT = ( 1 << ( Z_WIDTH - 1 ) ) - 1;

            var xInt = ( uint )( uncompressed.X * X_DOT ) & 0x7FF;
            var yInt = ( uint )( uncompressed.Y * Y_DOT ) & 0x7FF;
            var zInt = ( uint )( uncompressed.Z * Z_DOT ) & 0x3FF;

            var compressed = xInt << X_POS | yInt << Y_POS | zInt << Z_POS;

            return compressed;
        }

        public static uint CompressTangent( Vector3 uncompressed )
        {
            var x = ( byte )Math.Min( ( uncompressed.X * 127f ) + 127f, byte.MaxValue );
            var y = ( byte )Math.Min( ( uncompressed.Y * 127f ) + 127f, byte.MaxValue );
            var z = ( byte )Math.Min( ( uncompressed.Z * 127f ) + 127f, byte.MaxValue );
            var w = ( byte ) uncompressed.X < 1 ? 0x00 : 0xFF;
            return (uint)(x << 0 | y << 8 | z << 16 | w << 24 );
        }
    }
}
