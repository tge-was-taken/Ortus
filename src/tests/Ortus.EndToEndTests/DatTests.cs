using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ortus.Archives.Dat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ortus.EndToEndTests
{
    [TestClass]
    public class DatTests : TestBase
    {
        [TestMethod]
        public void dat_roundtrip()
        {
            var outPath = GetTempFilePath();

            try
            {
                using var dat = new DatArchive( GetFilePath( @"data000.cpk\pl\pl1010.dat" ) );
                dat.Save( outPath );
                using var dat2 = new DatArchive( outPath );
                Assert.AreEqual( dat.Files.Count, dat2.Files.Count );
                Assert.AreEqual( dat.MetadataId, dat2.MetadataId );
                CollectionAssert.AreEqual( dat.MetadataIndices, dat2.MetadataIndices );
                for ( int i = 0; i < dat.Files.Count; i++ )
                {
                    Assert.AreEqual( dat.Files[ i ].Name, dat2.Files[ i ].Name );
                    Assert.AreEqual( dat.Files[ i ].Type, dat2.Files[ i ].Type );
                    Assert.AreEqual( dat.Files[ i ].Size, dat2.Files[ i ].Size );
                    Assert.AreEqual( dat.Files[ i ].Hash, dat2.Files[ i ].Hash );
                    Assert.AreEqual( dat.Files[ i ].Id, dat2.Files[ i ].Id );
                }
            }
            finally
            {
                File.Delete( outPath );
            }
        }

        [TestMethod]
        public void add_file_to_dat()
        {
            var outPath = GetTempFilePath();

            try
            {
                using var dat = new DatArchive( GetFilePath( @"data000.cpk\pl\pl1010.dat" ) );
                dat.AddFile( GetFilePath( @"data000.cpk\CameraFactorEnemy.bxm" ) );
                dat.Save( outPath );
                using var dat2 = new DatArchive( outPath );
                Assert.AreEqual( dat.Files.Count, dat2.Files.Count );
                Assert.AreEqual( dat.MetadataId, dat2.MetadataId );
                CollectionAssert.AreEqual( dat.MetadataIndices, dat2.MetadataIndices );
                for ( int i = 0; i < dat.Files.Count; i++ )
                {
                    Assert.AreEqual( dat.Files[ i ].Name, dat2.Files[ i ].Name );
                    Assert.AreEqual( dat.Files[ i ].Type, dat2.Files[ i ].Type );
                    Assert.AreEqual( dat.Files[ i ].Size, dat2.Files[ i ].Size );
                    Assert.AreEqual( dat.Files[ i ].Hash, dat2.Files[ i ].Hash );
                    Assert.AreEqual( dat.Files[ i ].Id, dat2.Files[ i ].Id );
                }
            }
            finally
            {
                File.Delete( outPath );
            }
        }

        [TestMethod]
        public void replace_file_in_dat()
        {
            throw new NotImplementedException();
        }
    }
}
