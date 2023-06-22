using Newtonsoft.Json;
using Ortus.Models.Representation;
using System.Diagnostics;

var path = @"X:\code\repo\Ortus\Ortus.FileBatchProcessor\bin\Debug\net6.0\output\pl1400.wmb";
var model = new Ortus.Models.Wmb4.Model( path );
ExportObj( model );
ExportYaml( model );
ExportJson( model );

static void ExportJson( Ortus.Models.Wmb4.Model model )
{
    File.WriteAllText( "out.json", JsonConvert.SerializeObject( Model.FromAsset( model ), Formatting.Indented ) );
}

static void ExportYaml( Ortus.Models.Wmb4.Model model )
{
    File.WriteAllText( "out.yml", new YamlDotNet.Serialization.SerializerBuilder()
        .Build()
        .Serialize( Model.FromAsset( model ) ) );
}

static void ExportObj( Ortus.Models.Wmb4.Model model )
{
    using var writer = File.CreateText( "out.obj" );
    var groupVertexOffset = 0;

    for ( int groupIndex = 0; groupIndex < model.Groups.Count; groupIndex++ )
    {
        var group = model.Groups[ groupIndex ];
        writer.WriteLine( $"// group {group.Name} #{groupIndex}" );
        for ( int lod = 0; lod < group.LodMeshIndices.Length; lod++ )
        {
            writer.WriteLine( $"// lod #{lod}" );
            var meshIds = group.LodMeshIndices[ lod ];
            for ( int meshIdIndex = 0; meshIdIndex < meshIds.Count; meshIdIndex++ )
            {
                var meshId = meshIds[ meshIdIndex ];
                writer.WriteLine( $"// mesh {meshId} #{meshIdIndex}" );

                var mesh = model.Meshes[ lod ][ meshId ];
                Debug.Assert( mesh.GroupIndex == groupIndex );

                var subMesh = model.SubMeshes[ mesh.SubMeshIndex ];
                var bufferGroup = model.BufferGroups[ subMesh.BufferGroupIndex ];
                writer.WriteLine( $"// submesh #{mesh.SubMeshIndex}, buffergroup #{subMesh.BufferGroupIndex}" );

                writer.WriteLine( $"o group_{group.Name}_{groupIndex}_lod_{lod}_mesh_{meshId}_submesh_{mesh.SubMeshIndex}" );

                for ( int i = 0; i < subMesh.VertexCount; i++ )
                {
                    var vertex = bufferGroup.VertexBuffers[ 0 ][ subMesh.VertexBufferStartIndex + i ];
                    writer.WriteLine( $"v {vertex.Position.Value.X} {vertex.Position.Value.Y} {vertex.Position.Value.Z}" );
                }

                for ( int i = 0; i < subMesh.IndexCount / 3; i++ )
                {
                    var i1 = bufferGroup.IndexBuffer[ subMesh.IndexBufferStartIndex + ( i * 3 ) + 0 ] + groupVertexOffset + 1;
                    var i2 = bufferGroup.IndexBuffer[ subMesh.IndexBufferStartIndex + ( i * 3 ) + 1 ] + groupVertexOffset + 1;
                    var i3 = bufferGroup.IndexBuffer[ subMesh.IndexBufferStartIndex + ( i * 3 ) + 2 ] + groupVertexOffset + 1;
                    writer.WriteLine( $"f {i1} {i2} {i3}" );
                }

                groupVertexOffset += subMesh.VertexCount;
            }
        }
    }
}