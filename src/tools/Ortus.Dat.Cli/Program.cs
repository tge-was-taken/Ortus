
using Ortus.Archives.Dat;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

if ( args.Length == 0 )
{
    Console.WriteLine( $"datpack {Assembly.GetExecutingAssembly().GetName().Version} by tge" );
    Console.WriteLine( $"usage:" );
    Console.WriteLine( "  drag & drop dat/folder to unpack or pack" );
    Console.WriteLine( "alternatively:" );
    Console.WriteLine( "  datpack <input file path> [output file path]" );
    Console.ReadKey();
    return;
}

var path = args[ 0 ];
var outPath = args.Length > 1 ? args[ 1 ] : null;
if ( Directory.Exists( path ) )
{
    DatArchiveHelper.Pack( path, outPath );
}
else
{
    DatArchiveHelper.Unpack( path, outPath );
}