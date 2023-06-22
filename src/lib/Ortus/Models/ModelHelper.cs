namespace Ortus.Models
{
    //public static class ModelHelper
    //{
    //    public static void ReplaceModel( IModelConverter converter, string srcPath, string dstPath, string fbxPath )
    //    {
    //        // Replace model
    //        var id = Path.GetFileNameWithoutExtension( srcPath );
    //        var baseDir = Path.GetDirectoryName( dstPath );

    //        var tempWmbPath = Path.GetTempFileName();
    //        var tempWtaPath = Path.GetTempFileName();

    //        var archive = new DatArchive( srcPath );
    //        var model = new Model( archive.OpenFile( $"{id}.wmb" ), true );
    //        var newModelData = converter.Import( fbxPath, model );
    //        newModelData.Wmb.Save( tempWmbPath );
    //        newModelData.Dtt.Save( tempWtaPath, Path.Combine( baseDir, $"{id}.dtt" ) );

    //        // Replace files in archive
    //        archive.ReplaceFile( $"{id}.wmb", tempWmbPath );
    //        archive.ReplaceFile( $"{id}.wta", tempWtaPath );

    //        using ( var archiveStream = archive.Save() )
    //        using ( var fileStream = FileHelper.Create( Path.Combine( baseDir, $"{id}.dat" ) ) )
    //            archiveStream.CopyTo( fileStream );
    //    }
    //}
}
