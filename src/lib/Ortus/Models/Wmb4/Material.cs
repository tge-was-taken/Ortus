using Amicitia.IO.Binary;
using System;


namespace Ortus.Models.Wmb4
{
    public class Material : IBinarySerializable, ICloneable
    {
        public string ShaderName { get; set; }

        public List<int> TextureIndices { get; set; }

        public int Field08 { get; set; }

        public List<float> ShaderParameters { get; set; }

        public short Field10 { get; set; }

        public short Field12 { get; set; }

        public short Field14 { get; set; }

        public Material()
        {
            TextureIndices = new List<int>();
            ShaderParameters = new List<float>();
        }

        public object Clone()
        {
            var material = new Material();
            material.ShaderName = ShaderName;
            material.TextureIndices = TextureIndices.ToList();
            material.Field08 = Field08;
            material.ShaderParameters = ShaderParameters.ToList();
            material.Field10 = Field10;
            material.Field12 = Field12;
            material.Field14 = Field14;
            return material;
        }

        void IBinarySerializable.Read( BinaryObjectReader reader )
        {
            ShaderName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            var materialTextureMapListOffset = reader.ReadInt32();
            Field08 = reader.ReadInt32();
            var shaderParameterListOffset = reader.ReadInt32();
            Field10 = reader.ReadInt16();
            Field12 = reader.ReadInt16();
            Field14 = reader.ReadInt16();
            var shaderParameterCount = reader.ReadInt16();

            if ( materialTextureMapListOffset != 0 )
            {
                var size = shaderParameterListOffset - materialTextureMapListOffset;
                var count = size / 4;
                reader.ReadAtOffset( materialTextureMapListOffset, reader =>
                {
                    for ( int i = 0; i < count; i++ )
                        TextureIndices.Add( reader.ReadInt32() );
                } );
            }

            if ( shaderParameterListOffset != 0 )
            {
                reader.ReadAtOffset( shaderParameterListOffset, reader =>
                {
                    for ( int i = 0; i < shaderParameterCount; i++ )
                        ShaderParameters.Add( reader.ReadSingle() );
                } );
            }
        }

        void IBinarySerializable.Write( BinaryObjectWriter writer )
        {
            writer.WriteOffset( () => writer.WriteString( StringBinaryFormat.NullTerminated, ShaderName ), alignment: 16 );
            writer.WriteOffset( writer =>
            {
                for ( int i = 0; i < TextureIndices.Count; i++ )
                    writer.WriteInt32( TextureIndices[ i ] );
            }, alignment: 16 );
            writer.WriteInt32( Field08 );
            writer.WriteOffset( writer =>
            {
                for ( int i = 0; i < ShaderParameters.Count; i++ )
                    writer.WriteSingle( ShaderParameters[ i ] );
            }, alignment: 16 );
            writer.WriteInt16( Field10 );
            writer.WriteInt16( Field12 );
            writer.WriteInt16( Field14 );
            writer.WriteInt16( (short)ShaderParameters.Count );
        }
    }
}
