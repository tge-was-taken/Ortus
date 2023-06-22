using System;
using System.Diagnostics;
using System.Linq;

namespace Ortus.Models.Utilities
{
    public static class WeightCompressor
    {
        public static byte[] Compress( float[] weights )
        {
            var convertedWeights = weights.Select( x => ( byte )Math.Round( x * byte.MaxValue ) ).ToArray();
            var usedBoneCount    = convertedWeights.Count( x => x != 0 );

            Debug.Assert( usedBoneCount > 0 );

            if ( usedBoneCount == 1 )
            {
                convertedWeights[ 0 ] = 0xFF;
                return convertedWeights;
            }

            var total = convertedWeights.Sum( x => x );
            var error = byte.MaxValue - total;

            if ( error > 0 )
            {
                var errorAvg = ( byte )( Math.Max( 1, error / usedBoneCount ) );
                for ( var j = 0; j < usedBoneCount; j++ )
                {
                    convertedWeights[j] =  ( byte )Math.Min( ( int )( convertedWeights[j] + errorAvg ), byte.MaxValue );
                    error -= errorAvg;

                    Debug.Assert( error >= 0 );
                    if ( error == 0 )
                        break;
                }
            }
            else if ( error < 0 )
            {
                error = -error;
                var errorAvg = ( byte ) ( Math.Max( 1, error / usedBoneCount ) );

                for ( var j = 0; j < usedBoneCount; j++ )
                {
                    convertedWeights[j] =  ( byte )Math.Min( ( int )( convertedWeights[j] - errorAvg ), byte.MaxValue );
                    error  -= errorAvg;

                    Debug.Assert( error >= 0 );
                    if ( error == 0 )
                        break;
                }
            }

            return convertedWeights;
        }

        public static float[] Decompress( byte[] weights )
        {
            var decompressed = weights.Select( x => ( float ) x / ( float ) byte.MaxValue )
                                      .ToArray();

            var total = decompressed.Sum( x => x );
            if ( total < 1f )
            {
                var error = 1f - total;
                var usedBoneCount = weights.Count( x => x != 0 );
                var errorAvg = error / ( float ) usedBoneCount;

                for ( int i = 0; i < usedBoneCount; i++ )
                    decompressed[i] += errorAvg;
            }

            return decompressed;
        }
    }
}
