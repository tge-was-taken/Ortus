namespace Ortus.Xml
{
    public static class XmlHelper
    {
        public static bool TryConvert( string path, string? outPath = default )
        {
            if ( XmlDocument.IsValidBinaryFile( path ) )
            {
                var bxm = XmlDocument.FromBinary( path );
                bxm.SaveXml( outPath ?? Path.ChangeExtension( path, ".xml" ) );
                return true;
            }
            else if ( XmlDocument.IsValidXmlFile( path ) )
            {
                var bxm = XmlDocument.FromXml( path );
                bxm.SaveBinary( outPath ?? Path.ChangeExtension( path, ".bxm" ) );
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
