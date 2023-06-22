using Ortus.Utilities;
using System.IO;
using System.Text;

namespace Ortus.Textures
{
    public static class TextureHelper
    {
        public static uint GetHash( string name )
        {
            name = Path.GetFileNameWithoutExtension( name );
            if ( !uint.TryParse( name, out var hash ) )
                hash = Hasher.ComputeNameHash( name );

            return hash;
        }
    }
}
