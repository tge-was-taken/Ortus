using Ortus.Archives.Dat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ortus.Archives.Dat
{
    public static class DatArchiveHelper
    {
        public static void Pack( string path, string outPath = null )
        {
            var dat = new DatArchive();
            var manifestFilePath = Path.Combine( path, "!manifest.xml" );
            var processedFiles = new HashSet<string>();

            if ( File.Exists( manifestFilePath ) )
            {
                var manifest = Manifest.FromXmlFile( manifestFilePath );
                dat.Endianness = manifest.Endianness;
                dat.MetadataId = manifest.MetadataId;
                dat.MetadataIndices.AddRange( manifest.MetadataIndices );
                foreach ( var file in manifest.Files )
                {
                    var filePath = Path.Combine( path, file.Name );
                    if ( File.Exists( filePath ) )
                    {
                        processedFiles.Add( filePath.ToLower() );
                        dat.Files.Add( new ExternalFileEntry(
                            file.Name, 
                            file.Type ?? Path.GetExtension( file.Name ).Substring( 1 ), 
                            file.Hash, 
                            file.Id, 
                            File.OpenRead( filePath ) 
                            ) 
                        );
                    }
                }

                processedFiles.Add( manifestFilePath.ToLower() );

                if ( outPath == null && manifest.Name != null )
                    outPath = Path.Combine( Path.GetDirectoryName( path ), manifest.Name );
            }

            // Add remaining files not listed in the manifest
            foreach ( var file in Directory.EnumerateFiles( path ) )
            {
                if ( processedFiles.Contains( file.ToLower() ) )
                    continue;

                dat.AddFile( file );
            }

            if ( outPath == null )
            {
                if ( path.EndsWith( "_unpacked" ) )
                    outPath = path.Substring( 0, path.Length - 9 );
            }

            dat.Save( outPath ?? path + ".dat" );
        }

        public static void Unpack( string path, string outPath = null )
        {
            // Unpack
            var dat = new DatArchive( path );

            outPath = outPath ?? path + "_unpacked";
            Directory.CreateDirectory( outPath );

            foreach ( var file in dat.Files )
            {
                using ( var outFile = File.Create( Path.Combine( outPath, file.Name ) ) )
                    file.GetStream().CopyTo( outFile );
            }

            // Save manifest
            var manifest = new Manifest();
            manifest.Endianness = dat.Endianness;
            manifest.Name = Path.GetFileName( path );
            manifest.MetadataId = dat.MetadataId;
            manifest.MetadataIndices = dat.MetadataIndices;
            manifest.Files = dat.Files.Select( x => new FileEntryManifest()
            {
                Id = x.Id,
                Name = x.Name,
                Hash = x.Hash
            } ).ToList();
            manifest.SaveXmlFile( Path.Combine( outPath, "!manifest.xml" ) );
        }
    }
}
