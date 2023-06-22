using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Amicitia.IO.Binary;
using Ortus.Models.Utilities;

namespace Ortus.Models.Wmb4
{
    public class BufferGroup : IBinarySerializable<ModelFlags>
    {
        private const int VERTEX_BUFFER_COUNT = 4;
        private const int VERTEX_BONE_LIMIT = 4;

        public Vertex[][] VertexBuffers { get; }

        public ushort[] IndexBuffer { get; set; }

        public BufferGroup()
        {
            VertexBuffers = new Vertex[ VERTEX_BUFFER_COUNT ][];
            IndexBuffer = Array.Empty<ushort>();
        }

        void IBinarySerializable<ModelFlags>.Read( BinaryObjectReader reader, ModelFlags flags )
        {
            var vertexBufferOffsets = reader.ReadArray<int>( VERTEX_BUFFER_COUNT );
            var vertexCount = reader.ReadInt32();
            var indexBufferOffset = reader.ReadInt32();
            var indexCount = reader.ReadInt32();

            for ( int i = 0; i < vertexBufferOffsets.Length; i++ )
            {
                var offset = vertexBufferOffsets[ i ];
                if ( offset == 0 )
                    continue;

                var bufferIndex = i;
                reader.ReadAtOffset( offset, () =>
                {
                    VertexBuffers[ bufferIndex ] = new Vertex[ vertexCount ];

                    switch ( bufferIndex )
                    {
                        case 0:
                            for ( int j = 0; j < vertexCount; j++ )
                            {
                                var vertex = new Vertex();
                                vertex.Position = reader.Read<Vector3>();
                                vertex.UV = reader.ReadVector2Half();
                                vertex.Normal = reader.ReadVector3Packed();
                                vertex.Tangent = reader.ReadVector3Packed();

                                if ( flags.HasFlag( ModelFlags.HasWeights ) )
                                {
                                    vertex.BoneIndices = reader.ReadArray<byte>( VERTEX_BONE_LIMIT );
                                    vertex.BoneWeights = WeightCompressor.Decompress( reader.ReadArray<byte>( VERTEX_BONE_LIMIT ) );
                                }
                                else
                                {
                                    if ( flags.HasFlag( ModelFlags.HasColors ) )
                                        vertex.Color = reader.ReadUInt32();

                                    if ( flags.HasFlag( ModelFlags.HasTexCoord2 ) )
                                        vertex.UV2 = reader.ReadVector2Half();
                                }

                                VertexBuffers[ bufferIndex ][ j ] = vertex;
                            }

                            break;

                        case 1:
                            for ( int j = 0; j < vertexCount; j++ )
                            {
                                if ( !flags.HasFlag( ModelFlags.HasWeights ) )
                                    continue;

                                var vertex = new Vertex();

                                if ( flags.HasFlag( ModelFlags.HasColors ) )
                                    vertex.Color = reader.ReadUInt32();

                                if ( flags.HasFlag( ModelFlags.HasTexCoord2 ) )
                                    vertex.UV2 = reader.ReadVector2Half();

                                VertexBuffers[ bufferIndex ][ j ] = vertex;
                            }

                            break;

                        default:
                            throw new NotImplementedException( "More than 2 vertex buffers aren't handled yet" );
                    }
                } );
            }

            if ( indexBufferOffset != 0 )
                reader.ReadAtOffset( indexBufferOffset, () => IndexBuffer = reader.ReadArray<ushort>( indexCount ) );
        }

        void IBinarySerializable<ModelFlags>.Write( BinaryObjectWriter writer, ModelFlags flags )
        {
            for ( var i = 0; i < VertexBuffers.Length; i++ )
            {
                var buffer = VertexBuffers[ i ];
                if ( buffer == null || buffer.Length == 0 )
                {
                    writer.Write( 0 );
                    continue;
                }

                var bufferIndex = i;
                writer.WriteOffset( () =>
                {
                    var curBuffer = VertexBuffers[ bufferIndex ];

                    switch ( bufferIndex )
                    {
                        case 0:
                            foreach ( var vertex in curBuffer )
                            {
                                writer.Write( vertex.Position.Value );
                                writer.WriteVector2Half( vertex.UV.Value );
                                writer.WriteVector3Packed( vertex.Normal.Value );
                                writer.Write( VectorCompressor.Compress_11_11_10( vertex.Tangent.Value ) );

                                //writer.Write( VectorCompressor.CompressTangent( vertex.Tangent.Value ));

                                if ( flags.HasFlag( ModelFlags.HasWeights ) )
                                {
                                    writer.WriteArray( vertex.BoneIndices );
                                    var quantizedWeights = WeightCompressor.Compress( vertex.BoneWeights );
                                    writer.WriteArray( quantizedWeights );
                                }
                                else
                                {
                                    if ( flags.HasFlag( ModelFlags.HasColors ) )
                                        writer.Write( vertex.Color.Value );

                                    if ( flags.HasFlag( ModelFlags.HasTexCoord2 ) )
                                        writer.WriteVector2Half( vertex.UV2.Value );
                                }
                            }
                            break;

                        case 1:
                            foreach ( var vertex in curBuffer )
                            {
                                if ( flags.HasFlag( ModelFlags.HasColors ) )
                                    writer.Write( vertex.Color.Value );

                                if ( flags.HasFlag( ModelFlags.HasTexCoord2 ) )
                                    writer.WriteVector2Half( vertex.UV2.Value );
                            }
                            break;

                        default:
                            throw new NotImplementedException( "More than 2 vertex buffers aren't handled yet" );
                    }
                }, alignment: 16 );
            }

            var vertexCount = VertexBuffers.FirstOrDefault( x => x != null )?.Length ?? 0;
            writer.Write( vertexCount );

            if ( IndexBuffer != null && IndexBuffer.Length != 0 )
                writer.WriteOffset( () => writer.WriteArray( IndexBuffer ), alignment: 16 );
            else
                writer.Write( 0 );

            var indexCount = IndexBuffer?.Length ?? 0;
            writer.Write( indexCount );
        }
    }
}
