using Ortus.Textures.Utilities;
using System.Drawing;
using System.IO;

namespace Ortus.Textures.Utilities
{
    /// <summary>
    /// High level helper for DDS textures.
    /// </summary>
    public static class DDSHelper
    {
        /// <summary>
        /// Gets a stream containing DDS data. If the file is already in DDS format, it won't be re-encoded.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static Stream GetDDSStream( string filepath )
        {
            if ( Path.GetExtension( filepath ) == ".dds" )
            {
                return File.OpenRead( filepath );
            }
            else
            {
                var outFilePath = Path.ChangeExtension( filepath, ".dds" );
                TexConv.Run( filepath, outFilePath );
                return File.OpenRead( outFilePath );
            }
        }
    }
}
