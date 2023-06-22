namespace Ortus.Models
{
    public struct Color
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public uint RGBA
        {
            get
            {
                return (uint)( R << 24 | G << 16 | B << 8 | A );
            }
            set
            {
                FromRGBA( value );
            }
        }

        public uint ABGR
        {
            get
            {
                return (uint)( A << 24 | B << 16 | G << 8 | R );
            }
            set
            {
                FromABGR( value );
            }
        }

        public uint ARGB
        {
            get
            {
                return (uint)( A << 24 | R << 16 | G << 8 | B );
            }
            set
            {
                FromARGB( value );
            }
        }

        public static Color FromRGBA( uint rgba )
        {
            return new Color
            {
                R = (byte)( rgba & 0xFF000000 >> 24 ),
                G = (byte)( rgba & 0x00FF0000 >> 16 ),
                B = (byte)( rgba & 0x0000FF00 >> 8 ),
                A = (byte)( rgba & 0x000000FF )
            };
        }

        public static Color FromABGR( uint rgba )
        {
            return new Color
            {
                A = (byte)( rgba & 0xFF000000 >> 24 ),
                B = (byte)( rgba & 0x00FF0000 >> 16 ),
                G = (byte)( rgba & 0x0000FF00 >> 8 ),
                R = (byte)( rgba & 0x000000FF )
            };
        }

        public static Color FromARGB( uint rgba )
        {
            return new Color
            {
                A = (byte)( rgba & 0xFF000000 >> 24 ),
                R = (byte)( rgba & 0x00FF0000 >> 16 ),
                G = (byte)( rgba & 0x0000FF00 >> 8 ),
                B = (byte)( rgba & 0x000000FF )
            };
        }
    }
}
