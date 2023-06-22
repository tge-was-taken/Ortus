using System.Numerics;
using System.Runtime.CompilerServices;

namespace Ortus.Models.Converters.Assimp
{
    using Assimp = global::Assimp;

    public static class AssimpHelper
    {
        public static Vector2 ToVector2( this Assimp.Vector2D value ) => Unsafe.As<Assimp.Vector2D, Vector2>( ref value );

        public static Vector2 ToVector2( this Assimp.Vector3D value ) => new Vector2( value.X, value.Y );

        public static Vector3 ToVector3( this Assimp.Vector3D value ) => Unsafe.As<Assimp.Vector3D, Vector3>( ref value );

        public static uint ToUInt32( this Assimp.Color4D value ) => ( uint ) ( ( byte ) ( value.A * byte.MaxValue ) << 24 |
                                                                               ( byte ) ( value.R * byte.MaxValue ) << 16 |
                                                                               ( byte ) ( value.G * byte.MaxValue ) << 8 |
                                                                               ( byte ) ( value.B * byte.MaxValue ) );

        // ReSharper disable once InconsistentNaming
        public static Matrix4x4 ToMatrix4x4( this Assimp.Matrix4x4 value )
        {
            value.Transpose();
            return new Matrix4x4( value.A1, value.A2, value.A3, value.A4,
                                  value.B1, value.B2, value.B3, value.B4,
                                  value.C1, value.C2, value.C3, value.C4,
                                  value.D1, value.D2, value.D3, value.D4 );
        }
    }
}
