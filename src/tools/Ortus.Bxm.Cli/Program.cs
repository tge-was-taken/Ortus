using Ortus.Xml;
using System.Reflection;

if ( args.Length == 0 )
{
    Console.WriteLine( $"bxmconv {Assembly.GetExecutingAssembly().GetName().Version} by tge" );
    Console.WriteLine( $"usage:" );
    Console.WriteLine( "  drag & drop xml/bxm file to convert" );
    Console.WriteLine( "alternatively:" );
    Console.WriteLine( "  bxmconv <input file path> [output file path]" );
    Console.ReadKey();
    return;
}

var path = args[ 0 ];
var outPath = args.Length > 1 ? args[ 1 ] : null;
if ( !XmlHelper.TryConvert( path, outPath ) )
{
    Console.WriteLine( "unsupported file type: " + Path.GetExtension( path ) );
    Console.ReadKey();
    return;
}