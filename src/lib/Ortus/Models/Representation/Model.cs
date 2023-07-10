using Amicitia.IO.Binary;
using Amicitia.IO.Streams;
using Ortus.Models.Utilities;
using Ortus.Models.Wmb4;
using System.Diagnostics;

namespace Ortus.Models.Representation
{
    public class Model
    {
        public int Endian { get; set; }

        public ModelFlags Flags { get; set; }

        public PrimitiveType PrimitiveType { get; set; }

        public short Field0E { get; set; }

        public List<Bone> Bones { get; private set; }

        public ushort[] BoneIdToIndexTable { get; set; }

        public List<Material> Materials { get; private set; }

        public List<Texture> Textures { get; private set; }

        public List<Group> Groups { get; private set; }

        public bool HasWeights
        {
            get
            {
                return Groups
                    .SelectMany( x => x.Meshes )
                    .SelectMany( x => x.Vertices )
                    .Any( x => x.BoneWeights.Count > 0 );
            }
        }

        public bool HasColors
        {
            get
            {
                return Groups
                    .SelectMany( x => x.Meshes )
                    .SelectMany( x => x.Vertices )
                    .Any( x => x.Color.HasValue );
            }
        }

        public bool HasTexCoord2
        {
            get
            {
                return Groups
                    .SelectMany( x => x.Meshes )
                    .SelectMany( x => x.Vertices )
                    .Any( x => x.UV2.HasValue );
            }
        }

        public Model()
        {
            Bones = new List<Bone>();
            Materials = new List<Material>();
            Textures = new List<Texture>();
            Groups = new List<Group>();
        }

        public static Wmb4.Model ToWMB4( Model model )
        {
            var hasWeights = model.HasWeights;

            // TODO calculate
            var extents = new BoundingBox
            {
                Min = new System.Numerics.Vector3( -999999, -999999, -999999 ),
                Max = new System.Numerics.Vector3( 999999, 999999, 999999 )
            };

            var modelAsset = new Wmb4.Model
            {
                BoneIdToIndexTable = model.BoneIdToIndexTable,
                Endian = model.Endian,
                Flags = model.Flags,
                PrimitiveType = model.PrimitiveType,
                Field0E = model.Field0E,
                Extents = extents,
            };

            foreach ( var bone in model.Bones )
            {
                var parentIndex = model.Bones.FindIndex( y => y.Name == y.ParentName );
                var assetBone = new Wmb4.Bone
                {
                    Field08 = (ushort)bone.Field08,
                    Flags = (ushort)bone.Flags,
                    Id = (ushort)bone.Id,
                    Local = bone.Local,
                    World = bone.World,
                    ParentIndex = (ushort)( parentIndex == -1 ? ushort.MaxValue : parentIndex )
                };
                modelAsset.Bones.Add( assetBone );
            }

            foreach ( var mat in model.Materials )
            {
                var assetMat = new Wmb4.Material()
                {
                    Field08 = 0,
                    Field10 = (short)mat.Field10,
                    Field12 = (short)mat.Field12,
                    Field14 = 0,
                    ShaderName = mat.ShaderName,
                    ShaderParameters = mat.ShaderParameters,
                    TextureIndices = mat.TextureIndices,
                };
                modelAsset.Materials.Add( assetMat );
            }

            foreach ( var tex in model.Textures )
            {
                var assetTex = new Wmb4.TextureReference()
                {
                    Hash = (uint)tex.Hash,
                    Type = (uint)tex.Type,
                };
                modelAsset.Textures.Add( assetTex );
            }

            var indexBuffer = new List<int>();
            var vertexBuffer = new List<Wmb4.Vertex>();
            var vertexBuffer2 = new List<Wmb4.Vertex>();

            foreach ( var group in model.Groups )
            {
                var assetGroup = new Wmb4.Group()
                {
                    Extents = extents, // TODO calculate
                    Name = group.Name,
                };

                foreach ( var mesh in group.Meshes )
                {
                    var submesh = new SubMesh()
                    {
                        BufferGroupIndex = modelAsset.BufferGroups.Count,
                        IndexBufferStartIndex = indexBuffer.Count,
                        IndexCount = mesh.Indices.Count,
                        VertexBufferStartIndex = vertexBuffer.Count,
                        VertexCount = mesh.Vertices.Count,
                    };
                    indexBuffer.AddRange( mesh.Indices );

                    foreach ( var vertex in mesh.Vertices )
                    {
                        var assetVertex = new Wmb4.Vertex()
                        {
                            Normal = vertex.Normal,
                            Position = vertex.Position,
                            Tangent = vertex.Tangent,
                            UV = vertex.UV,
                        };

                        if ( hasWeights )
                        {
                            var assetVertex2 = new Wmb4.Vertex()
                            {
                                Color = vertex.Color.HasValue ? vertex.Color.Value.RGBA : null,
                                UV2 = vertex.UV2,
                            };
                            vertexBuffer2.Add( assetVertex2 );

                            for ( int i = 0; i < Math.Min(4, vertex.BoneWeights.Count); i++ )
                            {
                                assetVertex.BoneIndices[ i ] = (byte)vertex.BoneWeights[ i ].BoneIndex;
                                assetVertex.BoneWeights[ i ] = vertex.BoneWeights[ i ].Weight;
                            }
                        }
                        else
                        {
                            assetVertex.Color = vertex.Color.HasValue ? vertex.Color.Value.RGBA : null;
                            assetVertex.UV2 = vertex.UV2;
                        }
                    }

                    var materialIndex = (short)model.Materials.FindIndex( x => x.Name == mesh.MaterialName );
                    var assetMesh = new Wmb4.Mesh()
                    {
                        GroupIndex = modelAsset.Groups.Count,
                        MaterialIndex = materialIndex,
                        MatrixPaletteIndex = (short)modelAsset.MatrixPalettes.Count,
                        SubMeshIndex = modelAsset.SubMeshes.Count,
                        Field0C = 0x100,
                    };
                    assetGroup.LodMeshIndices[ mesh.LodLevel ].Add( (ushort)modelAsset.Meshes[ mesh.LodLevel ].Count );
                    assetGroup.MaterialIndices.Add( (ushort)materialIndex );
                    modelAsset.Meshes[ mesh.LodLevel ].Add( assetMesh );
                }

                modelAsset.Groups.Add( assetGroup );
            }

            return modelAsset;
        }

        public static Model FromWMB4( Wmb4.Model modelAsset )
        {
            var model = new Model();
            model.Endian = modelAsset.Endian;
            model.Flags = modelAsset.Flags;
            model.PrimitiveType = modelAsset.PrimitiveType;
            model.Field0E = modelAsset.Field0E;

            model.Bones.AddRange( modelAsset.Bones.Select( x => new Bone()
            {
                Name = $"node_{x.Id}",
                Field08 = x.Field08,
                Flags = x.Flags,
                Id = x.Id,
                ParentName = $"node_{x.ParentIndex}", // TODO fix this
                Local = x.Local,
            } ) );

            // TODO generate
            model.BoneIdToIndexTable = modelAsset.BoneIdToIndexTable.ToArray();

            model.Materials.AddRange( modelAsset.Materials.Select( x => new Material()
            {
                Name = $"mat_{modelAsset.Materials.IndexOf( x )}",
                Field10 = x.Field10,
                Field12 = x.Field12,
                ShaderName = x.ShaderName,
                ShaderParameters = x.ShaderParameters.ToList(),
                TextureIndices = x.TextureIndices.ToList(),
            } ) );

            model.Textures.AddRange( modelAsset.Textures.Select( x => new Texture()
            {
                Name = $"tex_{x.Hash:X8}.dds",
                Hash = x.Hash,
                Type = x.Type,
            } ) );

            foreach ( var groupB in modelAsset.Groups )
            {
                var groupA = new Group
                {
                    Name = groupB.Name,
                    Meshes = new List<Mesh>()
                };

                for ( int lod = 0; lod < groupB.LodMeshIndices.Length; lod++ )
                {
                    foreach ( var meshId in groupB.LodMeshIndices[ lod ] )
                    {
                        var meshB = modelAsset.Meshes[ lod ][ meshId ];
                        var subMesh = modelAsset.SubMeshes[ meshB.SubMeshIndex ];
                        var bufferGroup = modelAsset.BufferGroups[ subMesh.BufferGroupIndex ];

                        var meshA = new Mesh()
                        {
                            LodLevel = lod,
                            Indices = bufferGroup.IndexBuffer[ subMesh.IndexBufferStartIndex..( subMesh.IndexBufferStartIndex + subMesh.IndexCount ) ]
                                .Select( x => (int)x )
                                .ToList(),
                            MaterialName = "mat_" + meshB.MaterialIndex.ToString(),
                            Vertices = new List<Vertex>()
                        };

                        for ( int i = 0; i < subMesh.VertexCount; i++ )
                        {
                            var vertexB = bufferGroup.VertexBuffers[ 0 ][ subMesh.VertexBufferStartIndex + i ];
                            var vertexB2 = bufferGroup.VertexBuffers[ 1 ] != null ? bufferGroup.VertexBuffers[ 1 ][ subMesh.VertexBufferStartIndex + i ] : null;
                            var vertexA = new Vertex
                            {
                                Position = vertexB.Position.Value,
                                UV = vertexB.UV.Value,
                                Normal = vertexB.Normal.Value,
                                Tangent = vertexB.Tangent.Value,
                                BoneWeights = new List<VertexBoneWeight>()
                            };

                            if ( vertexB.BoneWeights != null )
                            {
                                for ( int j = 0; j < 4; j++ )
                                {
                                    if ( vertexB.BoneWeights[ j ] > 0 )
                                    {
                                        vertexA.BoneWeights.Add( new VertexBoneWeight()
                                        {
                                            BoneIndex = vertexB.BoneIndices[ j ],
                                            Weight = vertexB.BoneWeights[ j ]
                                        } );
                                    }
                                }
                            }

                            vertexA.Color = vertexB.Color.HasValue ? Color.FromRGBA( vertexB.Color.Value ) :
                                            vertexB2 != null && vertexB2.Color != null ? Color.FromRGBA( vertexB2.Color.Value ) :
                                            null;
                            vertexA.UV2 = vertexB.UV2.HasValue ? vertexB.UV2.Value :
                                            vertexB2 != null && vertexB2.UV2 != null ? vertexB2.UV2.Value :
                                            null;

                            meshA.Vertices.Add( vertexA );
                        }

                        groupA.Meshes.Add( meshA );
                    }
                }

                model.Groups.Add( groupA );
            }

            return model;
        }
    }
}
