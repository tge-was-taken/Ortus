using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ortus.Models.Wmb4
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ModelHeader
    {
        public int Magic;
        public int Endian;
        public int Flags;
        public short PrimitiveType;
        public short Field0E;
        public Vector3 Min;
        public Vector3 Max;
        public int BufferGroupListOffset;
        public int BufferGroupCount;
        public int SubMeshListOffset;
        public int SubMeshCount;
        public int MeshLodListOffset;
        public int NodeListOffset;
        public int NodeCount;
        public int NodeTranslationTableOffset;
        public int NodeTranslationTableSize;
        public int MatrixPaletteListOffset;
        public int MatrixPaletteCount;
        public int MaterialListOffset;
        public int MaterialCount;
        public int TextureListOffset;
        public int TextureCount;
        public int GroupListOffset;
        public int GroupCount;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public unsafe struct BufferGroupHeader
    {
        public fixed int VertexBufferOffsets[ 4 ];
        public int VertexCount;
        public int IndexBufferOffset;
        public int IndexCount;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public unsafe struct Vector2Half
    {
        public Half X;
        public Half Y;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public unsafe struct Vector3Packed
    {
        public int Bits;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public struct Color
    {
        public int Bits;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public unsafe struct VertexStatic
    {
        public Vector3 Position;
        public Vector2Half UV;
        public Vector3Packed Normal;
        public Vector3Packed Tangent;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public unsafe struct VertexSkin
    {
        public Vector3 Position;
        public Vector2Half UV;
        public Vector3Packed Normal;
        public Vector3Packed Tangent;
        public fixed byte BoneIndices[ 4 ];
        public fixed byte BoneWeights[ 4 ];
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public unsafe struct VertexStaticColor
    {
        public Vector3 Position;
        public Vector2Half UV;
        public Vector3Packed Normal;
        public Vector3Packed Tangent;
        public Color Color;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public unsafe struct VertexStaticUV2
    {
        public Vector3 Position;
        public Vector2Half UV;
        public Vector3Packed Normal;
        public Vector3Packed Tangent;
        public Vector2Half UV2;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public unsafe struct VertexStaticColorUV2
    {
        public Vector3 Position;
        public Vector2Half UV;
        public Vector3Packed Normal;
        public Vector3Packed Tangent;
        public Color Color;
        public Vector2Half UV2;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public unsafe struct Vertex2Color
    {
        public Color Color;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public unsafe struct Vertex2ColorUV
    {
        public Color Color;
        public Vector2Half UV;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public unsafe struct Vertex2UV
    {
        public Vector2Half UV;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public struct SubMeshHeader
    {
        public int BufferGroupIndex;
        public int VertexBufferStartIndex;
        public int IndexBufferStartIndex;
        public int VertexCount;
        public int TriangleCount;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public struct MeshHeader
    {
        public int SubMeshIndex;
        public int GroupIndex;
        public short MaterialIndex;
        public short MatrixPaletteIndex;
        public int Field0C;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public struct NodeData
    {
        public ushort Id;
        public ushort Flag;
        public ushort ParentIndex;
        public ushort Field08;
        public Vector3 Local;
        public Vector3 World;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public struct MaterialHeader
    {
        public int ShaderNameOffset;
        public int TextureMapListOffset;
        public int Field08;
        public int ParameterListOffset;
        public short Field10;
        public short Field12;
        public short Field14;
        public short ParameterCount;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public struct TextureReferenceData
    {
        public int Flags;
        public int Hash;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public unsafe struct GroupData
    {
        public int NameOffset;
        public Vector3 Min;
        public Vector3 Max;
    }

    [StructLayout( LayoutKind.Sequential, Pack = 1 )]
    public struct ListHeader
    {
        public int Offset;
        public int Count;
    }
}
