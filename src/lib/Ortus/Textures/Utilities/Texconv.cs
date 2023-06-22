using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ortus.Textures.Utilities
{
    public static class TexConv
    {
        public static void Run( string inPath, string outPath, 
            string fileType = "DDS", string featureLevel = "9.1", bool pow2 = true,
            string fmt = "DXT5", bool overwrite = true, bool srgb = true )
        {
            var startInfo = new ProcessStartInfo()
            {
                Arguments = $"\"{inPath}\" -o \"{outPath}\" -ft {fileType} -fl {featureLevel} -f {fmt}",
                CreateNoWindow = true,
                FileName = Path.Combine(
                    Path.GetFileNameWithoutExtension( Assembly.GetExecutingAssembly().Location ),
                    "texconv.exe" ),
                UseShellExecute = false,
            };
            if ( pow2 ) startInfo.Arguments += " -pow2";
            if ( overwrite ) startInfo.Arguments += " -y";
            if ( srgb ) startInfo.Arguments += " -srgb";

            var process = Process.Start( startInfo );
            process?.WaitForExit();
        }
    }
}
