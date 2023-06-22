using System.IO;

namespace Ortus.Archives
{
    public interface IEntry
    {
        uint Hash { get; set; }

        uint Size { get; }

        Stream GetStream();
    }
}