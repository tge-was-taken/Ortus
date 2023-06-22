using Ortus.Archives.Wtb;
using Ortus.Models.Utilities;
using Ortus.Models.Wmb4;
using Ortus.Textures;
using Ortus.Textures.Utilities;
using System.Diagnostics;
using System.Numerics;

namespace Ortus.Models.Converters.Assimp
{
    using Assimp = global::Assimp;


    public class AssimpModelConverter : IModelConverter
    {
        public void Export( Wmb4.ModelAssetBundle model, string filePath )
        {
            throw new NotImplementedException();
        }

        public Wmb4.ModelAssetBundle Import( string filePath, Wmb4.Model? reference = null )
        {
            var wmb = reference;

            // Use first material as base for now
            var baseMaterial = wmb.Materials[ 0 ];

            // Clear data of the original model
            wmb.BufferGroups.Clear();
            wmb.SubMeshes.Clear();
            Array.ForEach( wmb.Meshes, x => x.Clear() );
            wmb.MatrixPalettes.Clear();
            wmb.Groups.Clear();
            wmb.Textures.Clear();
            wmb.Materials.Clear();

            // Import scene
            var context = new Assimp.AssimpContext();
            context.SetConfig( new Assimp.Configs.FBXPreservePivotsConfig( false ) );
            var postProcessFlags =
                Assimp.PostProcessSteps.JoinIdenticalVertices |
                Assimp.PostProcessSteps.CalculateTangentSpace |
                Assimp.PostProcessSteps.FindDegenerates |
                Assimp.PostProcessSteps.FindInvalidData |
                Assimp.PostProcessSteps.FlipUVs |
                Assimp.PostProcessSteps.GenerateNormals |
                Assimp.PostProcessSteps.GenerateUVCoords |
                Assimp.PostProcessSteps.ImproveCacheLocality |
                Assimp.PostProcessSteps.Triangulate |
                Assimp.PostProcessSteps.FlipWindingOrder |
                Assimp.PostProcessSteps.FixInFacingNormals |
                Assimp.PostProcessSteps.JoinIdenticalVertices |
                Assimp.PostProcessSteps.LimitBoneWeights |
                Assimp.PostProcessSteps.OptimizeMeshes;
            var aiScene = context.ImportFile( filePath, postProcessFlags );

            var modelUsesWeights = aiScene.Meshes.Any( x => x.HasBones );
            var modelUsesColors = aiScene.Meshes.Any( x => x.HasVertexColors( 0 ) );
            var modelUsesTexCoord2 = aiScene.Meshes.Any( x => x.HasTextureCoords( 1 ) );
            var modelUsesVertexBuffer2 = modelUsesWeights && ( modelUsesColors || modelUsesTexCoord2 );

            if ( modelUsesWeights ) wmb.Flags |= ModelFlags.HasWeights; else wmb.Flags &= ~ModelFlags.HasWeights;
            if ( modelUsesColors ) wmb.Flags |= ModelFlags.HasColors; else wmb.Flags &= ~ModelFlags.HasColors;
            if ( modelUsesTexCoord2 ) wmb.Flags |= ModelFlags.HasTexCoord2; else wmb.Flags &= ~ModelFlags.HasTexCoord2;

            wmb.Flags &= ~ModelFlags.Bit8;

            var vertexBuffer = new List<Wmb4.Vertex>();

            List<Wmb4.Vertex> vertexBuffer2 = null;
            if ( modelUsesVertexBuffer2 )
                vertexBuffer2 = new List<Wmb4.Vertex>();

            var indexBuffer = new List<ushort>();
            var materials = new Dictionary<int, Wmb4.Material>();
            var textureData = new WtbArchive( DataStorageMode.External );
            var meshGroups = new Dictionary<string, Wmb4.Group>();

            void RecurseOverNodes( Assimp.Node node, ref Matrix4x4 parentTransform )
            {
                var transform = parentTransform * node.Transform.ToMatrix4x4();

                if ( node.HasMeshes )
                {
                    var tagName = TagName.Parse( node.Name );
                    Wmb4.Group group = null;
                    List<ushort> groupSlotIndices = null;
                    var lodLevel = 0;

                    foreach ( var property in tagName.Properties )
                    {
                        switch ( property.Name )
                        {
                            case "GRP":
                                {
                                    var groupName = property.Arguments[ 0 ];
                                    if ( !meshGroups.TryGetValue( groupName, out group ) )
                                        group = meshGroups[ groupName ] = new Wmb4.Group( groupName );

                                    groupSlotIndices = new List<ushort>();
                                    for ( int i = 1; i < property.Arguments.Count; i++ )
                                    {
                                        groupSlotIndices.Add( ushort.Parse( property.Arguments[ i ] ) );
                                    }
                                }
                                break;

                            case "LOD":
                                lodLevel = int.Parse( property.Arguments[ 0 ] );
                                break;
                        }
                    }

                    if ( group == null )
                    {
                        if ( !meshGroups.TryGetValue( tagName.Name, out group ) )
                            group = meshGroups[ tagName.Name ] = new Wmb4.Group( tagName.Name );

                        // Default slot indices for Raidens model
                        groupSlotIndices = new List<ushort>() { 0, 3, 4 };
                    }

                    foreach ( var aiMeshIndex in node.MeshIndices )
                    {
                        var aiMesh = aiScene.Meshes[ aiMeshIndex ];
                        if ( !aiMesh.HasVertices )
                            continue;

                        // Calculate all the things
                        var submeshIndex = wmb.SubMeshes.Count;
                        var meshGroupIndex = wmb.Groups.Count;
                        var bufferGroupIndex = wmb.BufferGroups.Count;
                        var vertexStartIndex = vertexBuffer.Count;
                        var indexStartIndex = indexBuffer.Count;
                        var triangleCount = aiMesh.FaceCount * 3;
                        var matrixPaletteIndex = modelUsesWeights ? (short)wmb.MatrixPalettes.Count : (short)-1;
                        var materialIndex = (byte)aiMesh.MaterialIndex;
                        var meshIndex = (ushort)wmb.Meshes[ lodLevel ].Count;

                        // Build submesh
                        wmb.SubMeshes.Add( new Wmb4.SubMesh( bufferGroupIndex, vertexStartIndex, indexStartIndex, aiMesh.VertexCount, triangleCount ) );

                        if ( !materials.ContainsKey( materialIndex ) )
                        {
                            // Build material
                            var aiMaterial = aiScene.Materials[ materialIndex ];
                            var material = (Wmb4.Material)baseMaterial.Clone();

                            // Diffuse
                            var diffuseTexturePath = aiMaterial.HasTextureDiffuse
                                ? Path.Combine( Path.GetDirectoryName( filePath ), aiMaterial.TextureDiffuse.FilePath )
                                : null;
                            var diffuseTextureHash = aiMaterial.HasTextureDiffuse
                                ? TextureHelper.GetHash( aiMaterial.TextureDiffuse.FilePath )
                                : DefaultTextures.DiffuseHash;
                            var diffuseTextureData = aiMaterial.HasTextureDiffuse && File.Exists( diffuseTexturePath )
                                ? DDSHelper.GetDDSStream( diffuseTexturePath )
                                : new MemoryStream( DefaultTextures.DiffuseData );

                            material.TextureIndices[1] = wmb.Textures.FindIndex( x => x.Hash == diffuseTextureHash );
                            if ( material.TextureIndices[1] == -1 )
                            {
                                material.TextureIndices[1] = wmb.Textures.Count;
                                wmb.Textures.Add( new Wmb4.TextureReference( diffuseTextureHash ) );
                                textureData.Textures.Add( new ExternalTextureEntry( diffuseTextureHash, diffuseTextureData ) );
                            }

                            // Normal
                            var normalTexturePath = aiMaterial.HasTextureNormal
                                ? Path.Combine( Path.GetDirectoryName( filePath ), aiMaterial.TextureNormal.FilePath )
                                : null;
                            var normalTextureHash = aiMaterial.HasTextureNormal
                                ? TextureHelper.GetHash( aiMaterial.TextureNormal.FilePath )
                                : DefaultTextures.NormalHash;
                            var normalTextureData = aiMaterial.HasTextureNormal && File.Exists( normalTexturePath )
                                ? DDSHelper.GetDDSStream( normalTexturePath )
                                : new MemoryStream( DefaultTextures.NormalData );

                            material.TextureIndices[7] = wmb.Textures.FindIndex( x => x.Hash == normalTextureHash );
                            if ( material.TextureIndices[ 7 ] == -1 )
                            {
                                material.TextureIndices[ 7 ] = wmb.Textures.Count;
                                wmb.Textures.Add( new Wmb4.TextureReference( normalTextureHash ) );
                                textureData.Textures.Add( new ExternalTextureEntry( normalTextureHash, normalTextureData ) );
                            }

                            materials[ materialIndex ] = material;
                        }

                        // Build mesh
                        wmb.Meshes[ lodLevel ].Add( new Wmb4.Mesh( submeshIndex, meshGroupIndex, materialIndex, matrixPaletteIndex ) );

                        // Add mesh to group
                        foreach ( var groupSlotIndex in groupSlotIndices )
                        {
                            if ( groupSlotIndex < group.LodMeshIndices.Length )
                                group.LodMeshIndices[ groupSlotIndex ].Add( meshIndex );
                        }

                        // Fill vertex buffers
                        for ( int i = 0; i < aiMesh.Vertices.Count; i++ )
                            vertexBuffer.Add( new Wmb4.Vertex() );

                        if ( modelUsesVertexBuffer2 )
                        {
                            for ( int i = 0; i < aiMesh.Vertices.Count; i++ )
                                vertexBuffer2.Add( new Wmb4.Vertex() );
                        }

                        var usedBones = new List<byte>();

                        // Convert vertices
                        for ( int i = 0; i < aiMesh.Vertices.Count; i++ )
                        {
                            var vertexIndex = vertexStartIndex + i;
                            var vertex = vertexBuffer[ vertexIndex ];

                            vertex.Position = Vector3.Transform( aiMesh.Vertices[ i ].ToVector3(), transform );
                            vertex.Normal = Vector3.TransformNormal( aiMesh.Normals[ i ].ToVector3(), transform );
                            vertex.Tangent = Vector3.TransformNormal( aiMesh.Tangents[ i ].ToVector3(), transform );
                            vertex.UV = aiMesh.HasTextureCoords( 0 ) ? aiMesh.TextureCoordinateChannels[ 0 ][ i ].ToVector2() : new Vector2();

                            bool TryAddUsedBone( string name, out byte index )
                            {
                                index = 0xFF;

                                byte nodeIndex;

                                if ( name.StartsWith( "Bone" ) )
                                {
                                    if ( !byte.TryParse( name.Substring( 4 ), out nodeIndex ) )
                                        return false;

                                    nodeIndex -= 1;
                                }
                                else
                                {
                                    var str = name;
                                    if ( str.StartsWith( "_" ) )
                                        str = str.Substring( 1 );

                                    if ( !int.TryParse( str, out var id ) )
                                        return false;

                                    var temp = wmb.Bones.FindIndex( x => x.Id == id );
                                    Trace.Assert( temp != -1 );
                                    nodeIndex = (byte)temp;
                                }

                                var curBoneIndex = usedBones.Count;

                                if ( usedBones.Contains( nodeIndex ) )
                                {
                                    curBoneIndex = usedBones.IndexOf( nodeIndex );
                                }
                                else
                                {
                                    usedBones.Add( nodeIndex );
                                }

                                index = (byte)curBoneIndex;
                                return true;
                            }

                            if ( modelUsesWeights )
                            {
                                if ( modelUsesVertexBuffer2 )
                                {
                                    var vertex2 = vertexBuffer2[ vertexIndex ];

                                    if ( modelUsesTexCoord2 )
                                        vertex2.UV2 = aiMesh.HasTextureCoords( 1 ) ? aiMesh.TextureCoordinateChannels[ 1 ][ i ].ToVector2() : vertex.UV;

                                    if ( modelUsesColors )
                                        vertex2.Color = aiMesh.HasVertexColors( 0 ) ? aiMesh.VertexColorChannels[ 0 ][ i ].ToUInt32() : uint.MaxValue;
                                }

                                vertex.BoneIndices = new byte[ 4 ];
                                vertex.BoneWeights = new float[ 4 ];

                                var affectedBones = aiMesh.Bones.Where( x => x.VertexWeights.Any( y => y.VertexID == i ) ).ToList();
                                if ( affectedBones.Count > 0 )
                                {
                                    for ( var boneIndex = 0; boneIndex < affectedBones.Count; boneIndex++ )
                                    {
                                        var bone = affectedBones[ boneIndex ];
                                        Trace.Assert( TryAddUsedBone( bone.Name, out byte index ) );

                                        var weights = bone.VertexWeights.Where( x => x.VertexID == i );
                                        var weight = weights.Sum( x => x.Weight );
                                        vertex.BoneIndices[ boneIndex ] = index;
                                        vertex.BoneWeights[ boneIndex ] = weight;
                                    }
                                }
                                else
                                {
                                    if ( !TryAddUsedBone( node.Name, out byte index ) )
                                    {
                                        if ( !usedBones.Contains( 0 ) )
                                        {
                                            index = (byte)usedBones.Count;
                                            usedBones.Add( 0 );
                                        }
                                        else
                                        {
                                            index = (byte)usedBones.IndexOf( 0 );
                                        }
                                    }

                                    vertex.BoneIndices[ 0 ] = index;
                                    vertex.BoneWeights[ 0 ] = 1.0f;
                                }
                            }
                            else
                            {
                                if ( modelUsesTexCoord2 )
                                    vertex.UV2 = aiMesh.HasTextureCoords( 1 ) ? aiMesh.TextureCoordinateChannels[ 1 ][ 1 ].ToVector2() : vertex.UV;

                                if ( modelUsesColors )
                                    vertex.Color = aiMesh.HasVertexColors( 0 ) ? aiMesh.VertexColorChannels[ 0 ][ i ].ToUInt32() : uint.MaxValue;
                            }
                        }

                        // Convert indices
                        for ( int i = 0; i < aiMesh.Faces.Count; i++ )
                        {
                            var aiFace = aiMesh.Faces[ i ];

                            for ( int j = 0; j < 3; j++ )
                                indexBuffer.Add( j < aiFace.IndexCount ? (ushort)aiFace.Indices[ j ] : (ushort)0 );
                        }

                        // Sort bone palette
                        if ( modelUsesWeights )
                        {
                            var sortedUsedBones = usedBones.OrderBy( x => x ).ToList();
                            for ( var j = 0; j < aiMesh.VertexCount; j++ )
                            {
                                var vertex = vertexBuffer[ vertexStartIndex + j ];
                                for ( var i = 0; i < 4; i++ )
                                {
                                    if ( vertex.BoneWeights[ i ] == 0 )
                                        continue;

                                    var nodeIndex = usedBones[ vertex.BoneIndices[ i ] ];
                                    vertex.BoneIndices[ i ] = (byte)sortedUsedBones.IndexOf( nodeIndex );
                                }
                            }

                            usedBones = sortedUsedBones;
                        }

                        if ( modelUsesWeights )
                        {
                            // Add bone palette
                            wmb.MatrixPalettes.Add( new Wmb4.MatrixPalette() { MatrixToNodeIndices = usedBones.ToArray() } );
                        }
                    }
                }

                foreach ( var child in node.Children )
                    RecurseOverNodes( child, ref transform );
            }

            // Recurse over scene
            var rootParentTransform = Matrix4x4.Identity;
            RecurseOverNodes( aiScene.RootNode, ref rootParentTransform );

            // Add buffer group
            var bufferGroup = new Wmb4.BufferGroup();
            bufferGroup.VertexBuffers[ 0 ] = vertexBuffer.ToArray();

            if ( modelUsesVertexBuffer2 )
                bufferGroup.VertexBuffers[ 1 ] = vertexBuffer2.ToArray();

            bufferGroup.IndexBuffer = indexBuffer.ToArray();
            wmb.BufferGroups.Add( bufferGroup );

            // Calculate extents
            wmb.Extents = BoundingBox.Calculate( vertexBuffer.Select( x => x.Position.Value ) );

            // Add mesh groups
            foreach ( var group in meshGroups.Values )
            {
                // TODO
                group.Extents = wmb.Extents;
                wmb.Groups.Add( group );
            }

            // Add materials
            wmb.Materials.AddRange( materials.Select( x => x.Value ) );

            return new Wmb4.ModelAssetBundle()
            {
                Wmb = wmb,
                Dtt = textureData
            };
        }
    }
}
