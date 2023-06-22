namespace Ortus.Models.Representation
{
    public class Material
    {
        public string Name { get; set; }
        public string ShaderName { get; set; }
        public int Field10 { get; set; }
        public int Field12 { get; set; }
        public List<int> TextureIndices { get; set; }
        public List<float> ShaderParameters { get; set; }
    }
}
