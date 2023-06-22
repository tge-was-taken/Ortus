using Ortus.Archives;
using Ortus.Archives.Dat;
using Ortus.Archives.Wtb;
using Ortus.Textures;
using Ortus.Utilities;

namespace Ortus.Models.Wmb4
{
    public class ModelAssetBundle
    {
        public Model Wmb { get; set; }
        public WtbArchive Dtt { get; set; }
    }

    public interface IModelConverter
    {
        ModelAssetBundle Import( string filePath, Model? reference = default );

        void Export( ModelAssetBundle model, string filePath );
    }

    public static class ModelHelper
    {
        public static void ReplaceModel( IModelConverter converter, string srcPath, string dstPath, string fbxPath )
        {
            // Replace model
            var id = Path.GetFileNameWithoutExtension( srcPath );
            var baseDir = Path.GetDirectoryName( dstPath );

            var tempWmbPath = Path.GetTempFileName();
            var tempWtaPath = Path.GetTempFileName();

            var archive = new DatArchive( srcPath );
            var model = new Model( archive.OpenFile( $"{id}.wmb" ), true );
            var newModelData = converter.Import( fbxPath, model );
            newModelData.Wmb.Save( tempWmbPath );
            newModelData.Dtt.Save( tempWtaPath, Path.Combine( baseDir, $"{id}.dtt" ) );

            // Replace files in archive
            archive.ReplaceFile( $"{id}.wmb", tempWmbPath );
            archive.ReplaceFile( $"{id}.wta", tempWtaPath );

            using ( var archiveStream = archive.Save() )
            using ( var fileStream = FileHelper.Create( Path.Combine( baseDir, $"{id}.dat" ) ) )
                archiveStream.CopyTo( fileStream );
        }
    }
}
