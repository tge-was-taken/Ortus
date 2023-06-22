using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ortus.Models.Converters.Assimp;
using Ortus.Models.Wmb4;
using System.IO;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Ortus.EndToEndTests
{
    [TestClass]
    public class WmbTests : TestBase
    {
        [TestMethod]
        public void wmb_roundtrip()
        {
            var tempFile = GetTempFilePath();
            try
            {
                var wmb = new Model( GetFilePath( @"data000.cpk\pl\pl1010\pl1010.wmb" ) );
                wmb.Save( tempFile );
                var wmb2 = new Model( tempFile );
                AssertWmbEquality( wmb, wmb2 );
            }
            finally
            {
                File.Delete( tempFile );
            }
        }

        [TestMethod]
        public void replace_wmb_with_fbx()
        {
            var tempWtbFile = GetTempFilePath();
            var tempWmbFile = GetTempFilePath();
            var tempWtaFile = GetTempFilePath();

            try
            {
                var wmb = new Model( GetFilePath( @"data000.cpk\pl\pl1010\pl1010.wmb" ) );
                var io = new AssimpModelConverter();
                var newModelData = io.Import( GetFilePath( "text.fbx" ), wmb );
                newModelData.Dtt.Save( tempWtbFile, tempWtaFile );
                newModelData.Wmb.Save( tempWmbFile );
                var wmb2 = new Model( tempWmbFile );
                AssertWmbEquality( wmb, wmb2 );
            }
            finally
            {
                File.Delete( tempWtbFile );
                File.Delete( tempWmbFile );
                File.Delete( tempWtaFile );
            }
        }

        private static void AssertWmbEquality( Model wmb, Model wmb2 )
        {
            AreEqual( wmb.Endian, wmb2.Endian );
            AreEqual( wmb.Flags, wmb2.Flags );
            AreEqual( wmb.PrimitiveType, wmb2.PrimitiveType );
            AreEqual( wmb.Field0E, wmb2.Field0E );
            AreEqual( wmb.Extents, wmb2.Extents );

            AreEqual( wmb.BufferGroups.Count, wmb2.BufferGroups.Count );
            for ( int i = 0; i < wmb.BufferGroups.Count; i++ )
            {
                var a = wmb.BufferGroups[ i ];
                var b = wmb2.BufferGroups[ i ];
                AreEqual( a.VertexBuffers.Length, b.VertexBuffers.Length );
                for ( int j = 0; j < a.VertexBuffers.Length; j++ )
                {
                    if ( a.VertexBuffers[ j ] == null ) continue;
                    for ( int k = 0; k < a.VertexBuffers[ j ].Length; k++ )
                    {
                        var x = a.VertexBuffers[ j ][ k ];
                        var y = b.VertexBuffers[ j ][ k ];
                        //AreEqual( true, x.Equals( y ) );
                    }
                }
                CollectionAssert.AreEqual( a.IndexBuffer, b.IndexBuffer );
            }

            AreEqual( wmb.SubMeshes.Count, wmb2.SubMeshes.Count );
            for ( int i = 0; i < wmb.SubMeshes.Count; i++ )
            {
                // TODO
            }

            AreEqual( wmb.Meshes.Length, wmb2.Meshes.Length );
            for ( int i = 0; i < wmb.Meshes.Length; i++ )
            {
                // TODO
            }

            AreEqual( wmb.Bones.Count, wmb2.Bones.Count );
            for ( int i = 0; i < wmb.Bones.Count; i++ )
            {
                // TODO
            }

            CollectionAssert.AreEqual( wmb.BoneIdToIndexTable, wmb2.BoneIdToIndexTable );

            AreEqual( wmb.MatrixPalettes.Count, wmb2.MatrixPalettes.Count );
            for ( int i = 0; i < wmb.MatrixPalettes.Count; i++ )
            {
                // TODO
            }

            AreEqual( wmb.Materials.Count, wmb2.Materials.Count );
            for ( int i = 0; i < wmb.Materials.Count; i++ )
            {
                // TODO
            }

            AreEqual( wmb.Textures.Count, wmb2.Textures.Count );
            for ( int i = 0; i < wmb.Textures.Count; i++ )
            {
                // TODO
            }

            AreEqual( wmb.Groups.Count, wmb2.Groups.Count );
            for ( int i = 0; i < wmb.Groups.Count; i++ )
            {
                // TODO
            }
        }
    }
}