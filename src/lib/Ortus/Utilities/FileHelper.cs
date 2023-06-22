using System.IO;

namespace Ortus.Utilities
{
    public class FileHelper
    {
        public static FileStream Create( string path )
        {
            path = Path.GetFullPath( path );

            if ( !Directory.Exists( path ) )
                Directory.CreateDirectory( Path.GetDirectoryName( path ) );

            return File.Create( path );
        }
    }
}
