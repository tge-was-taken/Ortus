using Force.Crc32;
using System;

namespace Ortus.Textures
{
    public static class DefaultTextures
    {
        private const string DEFAULT_DIFFUSE_TEXTURE = "RERTIHwAAAAHEAoABAAAAAQAAAAIAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAAAAEAAAARFhUMQAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAc+Bz4AAAAAA==";
        private const string DEFAULT_NORMAL_TEXTURE = "RERTIHwAAAAHEAoABAAAAAQAAAAIAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAAAAEAAAARFhUMQAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAAAAAAAAAAAAAAAAAAAAD/g/+DAAAAAA==";

        private static Lazy<uint> sDiffuseTextureHash { get; } = new Lazy<uint>( () => TextureHelper.GetHash( DEFAULT_DIFFUSE_TEXTURE ) );
        private static Lazy<byte[]> sDiffuseTextureData { get; } = new Lazy<byte[]>( () => Convert.FromBase64String( DEFAULT_DIFFUSE_TEXTURE ) );
        private static Lazy<uint> sNormalTextureHash { get; } = new Lazy<uint>( () => TextureHelper.GetHash( DEFAULT_NORMAL_TEXTURE ) );
        private static Lazy<byte[]> sNormalTextureData { get; } = new Lazy<byte[]>( () => Convert.FromBase64String( DEFAULT_NORMAL_TEXTURE ) );

        public static uint DiffuseHash => sDiffuseTextureHash.Value;

        public static byte[] DiffuseData => sDiffuseTextureData.Value;

        public static uint NormalHash => sNormalTextureHash.Value;

        public static byte[] NormalData => sNormalTextureData.Value;
    }
}
