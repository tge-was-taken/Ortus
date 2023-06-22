using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ortus.EndToEndTests
{
    public abstract class TestBase
    {
        protected const string FileRoot = @"X:\project\mgr\dumps";

        protected string GetFilePath( string path ) => Path.Combine( FileRoot, path );
        protected Stream OpenFile( string path ) => File.OpenRead( GetFilePath( path ) );
        protected byte[] ReadFile( string path ) => File.ReadAllBytes( GetFilePath( path ) );
        protected string GetTempFilePath() => Path.GetTempFileName();
    }
}
