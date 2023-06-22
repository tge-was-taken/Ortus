using System.Numerics;
using Amicitia.IO.Binary;
using Amicitia.IO.Streams;
using Ortus.Utilities;
using Ortus.Models.Utilities;
using Amicitia.IO.Binary.Extensions;

namespace Ortus.Models.Wmb4
{
    public class Model : IBinarySerializable
    {
        private const int MAGIC = 0x34424D57;
        private const int MESH_LOD_COUNT = 4;

        public int Endian { get; set; }

        public ModelFlags Flags { get; set; }

        public PrimitiveType PrimitiveType { get; set; }

        public short Field0E { get; set; }

        public BoundingBox Extents { get; set; }

        public List<BufferGroup> BufferGroups { get; private set; }

        public List<SubMesh> SubMeshes { get; private set; }

        public List<Mesh>[] Meshes { get; private set; }

        public List<Bone> Bones { get; private set; }

        public ushort[] BoneIdToIndexTable { get; set; }

        public List<MatrixPalette> MatrixPalettes { get; private set; }

        public List<Material> Materials { get; private set; }

        public List<TextureReference> Textures { get; private set; }

        public List<Group> Groups { get; private set; }

        public Model()
        {
            Initialize();
        }

        public Model( string filepath ) : this( File.OpenRead( filepath ), false ) { }

        public Model( Stream stream, bool leaveOpen )
        {
            Initialize();

            using ( var reader = new BinaryObjectReader( stream,
                leaveOpen ? StreamOwnership.Retain : StreamOwnership.Transfer,
                Endianness.Little ) )
                Read( reader );

            //CalculateTangents();
        }

        private void CalculateTangents()
        {
            var vertices = BufferGroups[ 0 ].VertexBuffers[ 0 ].Select( x => x.Position.Value ).ToArray();
            var normals = BufferGroups[ 0 ].VertexBuffers[ 0 ].Select( x => x.Normal.Value ).ToArray();
            var texCoords = BufferGroups[ 0 ].VertexBuffers[ 0 ].Select( x => x.UV.Value ).ToArray();
            CalculateTangentArray( vertices, normals, texCoords, BufferGroups[ 0 ].IndexBuffer, out var tangents );
        }

        private static void CalculateTangentArray( Vector3[] vertex, Vector3[] normal, Vector2[] texcoord, ushort[] triangle, out Vector4[] tangent )
        {
            Vector3[] tan1 = new Vector3[ vertex.Length ];
            Vector3[] tan2 = new Vector3[ vertex.Length ];

            for ( var a = 0; a < triangle.Length; a += 3 )
            {
                var i1 = triangle[ a + 2 ];
                var i2 = triangle[ a + 1 ];
                var i3 = triangle[ a + 0 ];

                var v1 = vertex[ i1 ];
                var v2 = vertex[ i2 ];
                var v3 = vertex[ i3 ];

                var w1 = texcoord[ i1 ];
                var w2 = texcoord[ i2 ];
                var w3 = texcoord[ i3 ];

                float x1 = v2.X - v1.X;
                float x2 = v3.X - v1.X;
                float y1 = v2.Y - v1.Y;
                float y2 = v3.Y - v1.Y;
                float z1 = v2.Z - v1.Z;
                float z2 = v3.Z - v1.Z;

                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;

                float r = 1.0F / ( s1 * t2 - s2 * t1 );
                var sdir = new Vector3( ( t2 * x1 - t1 * x2 ) * r, ( t2 * y1 - t1 * y2 ) * r,
                               ( t2 * z1 - t1 * z2 ) * r );
                var tdir = new Vector3( ( s1 * x2 - s2 * x1 ) * r, ( s1 * y2 - s2 * y1 ) * r,
                               ( s1 * z2 - s2 * z1 ) * r );

                tan1[ i1 ] += sdir;
                tan1[ i2 ] += sdir;
                tan1[ i3 ] += sdir;

                tan2[ i1 ] += tdir;
                tan2[ i2 ] += tdir;
                tan2[ i3 ] += tdir;
            }

            tangent = new Vector4[ vertex.Length ];

            for ( var a = 0; a < vertex.Length; a++ )
            {
                var n = normal[ a ];
                var t = tan1[ a ];

                // Gram-Schmidt orthogonalize
                tangent[ a ] = new Vector4( Vector3.Normalize( t - n * Vector3.Dot( n, t ) ), 0 );

                // Calculate handedness
                tangent[ a ].W = Vector3.Dot( Vector3.Cross( n, t ), tan2[ a ] ) < 0.0F ? -1.0F : 1.0F;
            }
        }

        private void Initialize()
        {
            Endian = 0;
            Flags = ModelFlags.Bit0 | ModelFlags.Bit1 | ModelFlags.Bit2;
            PrimitiveType = PrimitiveType.TriangleList;
            Field0E = -1;
            BufferGroups = new List<BufferGroup>();
            SubMeshes = new List<SubMesh>();
            Meshes = new List<Mesh>[ MESH_LOD_COUNT ];
            Bones = new List<Bone>();
            BoneIdToIndexTable = null;
            MatrixPalettes = new List<MatrixPalette>();
            Materials = new List<Material>();
            Textures = new List<TextureReference>();
            Groups = new List<Group>();
        }

        public void Save( Stream stream, bool leaveOpen )
        {
            using ( var writer = new BinaryObjectWriter( stream,
                leaveOpen ? StreamOwnership.Retain : StreamOwnership.Transfer,
                Endianness.Little ) )
            {
                Write( writer );
                writer.Flush();
            }
        }

        public void Save( string filepath ) => Save( FileHelper.Create( filepath ), false );

        public MemoryStream Save()
        {
            var stream = new MemoryStream();
            Save( stream, true );
            return stream;
        }

        internal void Read( BinaryObjectReader reader )
        {
            var magic = reader.ReadInt32();
            if ( magic != MAGIC )
                throw new InvalidFileFormatException( "Header magic value does not match expected value" );

            Endian = reader.ReadInt32();
            Flags = (ModelFlags)reader.ReadInt32();
            PrimitiveType = (PrimitiveType)reader.ReadInt16();
            Field0E = reader.ReadInt16();
            Extents = reader.Read<BoundingBox>();
            var bufferGroupListOffset = reader.ReadInt32();
            var bufferGroupCount = reader.ReadInt32();
            var subMeshListOffset = reader.ReadInt32();
            var subMeshCount = reader.ReadInt32();
            var meshLodListOffset = reader.ReadInt32();
            var boneListOffset = reader.ReadInt32();
            var nodeCount = reader.ReadInt32();
            var nodeHierarchyMapOffset = reader.ReadInt32();
            var nodeHierarchyMapSize = reader.ReadInt32();
            var matrixPaletteListOffset = reader.ReadInt32();
            var matrixPaletteCount = reader.ReadInt32();
            var materialListOffset = reader.ReadInt32();
            var materialCount = reader.ReadInt32();
            var textureListOffset = reader.ReadInt32();
            var textureCount = reader.ReadInt32();
            var meshGroupListOffset = reader.ReadInt32();
            var meshGroupCount = reader.ReadInt32();

            reader.ReadAtOffset( bufferGroupListOffset, reader =>
            {
                for ( int i = 0; i < bufferGroupCount; i++ )
                    BufferGroups.Add( reader.ReadObject<BufferGroup, ModelFlags>( Flags ) );
            } );

            reader.ReadAtOffset( subMeshListOffset, reader =>
            {
                for ( int i = 0; i < subMeshCount; i++ )
                    SubMeshes.Add( reader.ReadObject<SubMesh>() );
            } );

            reader.ReadAtOffset( meshLodListOffset, reader =>
            {
                for ( int i = 0; i < MESH_LOD_COUNT; i++ )
                {
                    var meshListOffset = reader.ReadInt32();
                    var meshCount = reader.ReadInt32();
                    var meshList = new List<Mesh>();
                    reader.ReadAtOffset( meshListOffset, reader =>
                    {
                        for ( int j = 0; j < meshCount; j++ )
                            meshList.Add( reader.ReadObject<Mesh>() );
                    } );
                    Meshes[ i ] = meshList;
                }
            } );

            reader.ReadAtOffset( boneListOffset, reader =>
            {
                for ( int i = 0; i < nodeCount; i++ )
                    Bones.Add( reader.ReadObject<Bone>() );
            } );

            reader.ReadAtOffset( nodeHierarchyMapOffset, reader =>
            {
                BoneIdToIndexTable = reader.ReadArray<ushort>( nodeHierarchyMapSize / 2 );
            } );

            reader.ReadAtOffset( matrixPaletteListOffset, reader =>
            {
                for ( int i = 0; i < matrixPaletteCount; i++ )
                    MatrixPalettes.Add( reader.ReadObject<MatrixPalette>() );
            } );

            reader.ReadAtOffset( materialListOffset, reader =>
            {
                for ( int i = 0; i < materialCount; i++ )
                {
                    Materials.Add( reader.ReadObject<Material>() );
                }
            } );

            reader.ReadAtOffset( textureListOffset, reader =>
            {
                for ( int i = 0; i < textureCount; i++ )
                    Textures.Add( reader.ReadObject<TextureReference>() );
            } );

            reader.ReadAtOffset( meshGroupListOffset, reader =>
            {
                for ( int i = 0; i < meshGroupCount; i++ )
                {
                    Groups.Add( reader.ReadObject<Group>() );
                }
            } );
        }

        internal void Write( BinaryObjectWriter writer )
        {
            void WriteList<T>( IList<T> list ) where T : IBinarySerializable
            {
                writer.WriteOffset( writer =>
                {
                    foreach ( var item in list )
                        writer.WriteObject( item );
                } );

                writer.Write( list.Count );
            }

            void WriteList2<T, T2>( IList<T> list, T2 context ) where T : IBinarySerializable<T2>
            {
                writer.WriteOffset( writer =>
                {
                    foreach ( var item in list )
                        writer.WriteObject( item, context );
                } );

                writer.Write( list.Count );
            }

            writer.Write( MAGIC );
            writer.Write( Endian );
            writer.Write( (int)Flags );
            writer.Write( (short)PrimitiveType );
            writer.Write( Field0E );
            writer.Write( Extents );
            WriteList2( BufferGroups, Flags );
            WriteList( SubMeshes );
            writer.WriteOffset( () => Array.ForEach( Meshes, x => WriteList( x ) ) );
            WriteList( Bones );
            writer.WriteOffset( () => Array.ForEach( BoneIdToIndexTable, writer.Write ) );
            writer.Write( BoneIdToIndexTable.Length * 2 );
            WriteList( MatrixPalettes );
            WriteList( Materials );
            WriteList( Textures );
            WriteList( Groups );
            writer.Align( 32 );
        }

        void IBinarySerializable.Read( BinaryObjectReader reader ) => Read( reader );
        void IBinarySerializable.Write( BinaryObjectWriter writer ) => Write( writer );
    }
}
