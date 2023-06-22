using Ortus;
using Ortus.Archives.Dat;
using Ortus.Archives.Wtb;
using Ortus.Models.Wmb4;
using Ortus.Xml;
using System.Diagnostics;
using CommandLine;
using Amicitia.IO;

var options = Parser.Default.ParseArguments<Options>( args )
    .MapResult( options =>
    {
        var assets = EnumerateAssets( options ).ToList();

        Directory.CreateDirectory( "output" );
        if ( options.ExtractFiles )
            ExtractFiles( options, assets );

        if ( options.DumpMaterialInfo )
            DumpMaterialInfo( options, assets );

        if ( options.DumpModelTextureInfo )
            DumpModelTextureInfo( options, assets );

        if ( options.DumpTextureInfo)
            DumpTextureInfo( options, assets );

        return 1;
    },
    error =>
    {
        return 0;
    } );

static void ExtractFiles( Options options, IEnumerable<AssetInfo> assets )
{
    var textureTypes = new Dictionary<uint, uint>();
    //foreach (var texture in assets
    //    .Where( x => x.Name.EndsWith( ".wmb" ) && x.Data != null )
    //    .Select( x => (Model)x.Data )
    //    .SelectMany( x => x.Textures ) )
    //{
    //    if ( textureTypes.ContainsKey( texture.Hash ) )
    //        Trace.Assert( textureTypes[ texture.Hash ] == texture.Type );
    //    textureTypes[ texture.Hash ] = texture.Type;
    //}

    foreach ( var asset in assets )
    {
        if ( asset.Stream is FileStream || asset.Stream.Length == 0 ) continue;

        if ( string.IsNullOrEmpty( asset.Name ) )
        {
            if ( asset.Tag is ITextureEntry texture )
            {
                Console.WriteLine( asset.Parent.Name );
                try
                {
                    string name = null;
                    if ( textureTypes.TryGetValue( texture.Hash, out var type ) )
                    {
                        name = $"output/tex_{Path.GetFileNameWithoutExtension( asset.Parent.Name )}_hash_{texture.Hash:X8}_flags_{texture.Flags:X8}_type_{type}.dds";
                    }
                    else
                    {
                        name = $"output/tex_{Path.GetFileNameWithoutExtension( asset.Parent.Name )}_hash_{texture.Hash:X8}_flags_{texture.Flags:X8}.dds";
                    }

                    Console.WriteLine( name );

                    using var fileStream = File.Create( name );
                    asset.Stream.CopyTo( fileStream );
                }
                catch ( Exception ex )
                {
                    Console.WriteLine( ex );
                }
            }
        }

        //if ( !File.Exists( $"output/{asset.Name}" ) )
        //{
        //    using var fileStream = File.Create( $"output/{asset.Name}" );
        //    asset.Stream.CopyTo( fileStream );
        //}
    }
}

static IEnumerable<AssetInfo> EnumerateAssets( Options options )
{
    static IEnumerable<AssetInfo> EnumerateAssetsInternal( Options options, AssetInfo asset )
    {
        yield return asset;

        if ( asset.Stream.Length != 0 )
        {
            var ext = Path.GetExtension( asset.Name )?.ToLower();
            if ( ext == ".dat" || ext == ".dtt" || ext == ".evn" )
            {
                asset.Data = null;
                try
                {
                    asset.Data = new DatArchive( asset.Stream );
                }
                catch ( Exception )
                {
                }

                if ( asset.Data != null )
                {
                    foreach ( var subFile in ((DatArchive)asset.Data).Files )
                    {
                        foreach ( var item in EnumerateAssetsInternal( options, 
                            new AssetInfo
                            {
                                Name = subFile.Name,
                                Stream = subFile.GetStream(),
                                Parent = asset,
                                Tag = subFile,
                            } ) )
                        {
                            yield return item;
                        }
                    }
                }
            }
            else if ( ext == ".wtb" || ext == ".wta" )
            {
                WtbArchive data = null;
                bool success = false;
                try
                {
                    data = new WtbArchive( asset.Stream );
                    if ( data.StorageMode == DataStorageMode.External )
                    {
                        if ( asset.Parent != null )
                        {
                            var dataStorageName = asset.Parent.Name
                                .Replace( ".dat", ".dtt" );
                            if ( !File.Exists( dataStorageName ) )
                            {
                                dataStorageName = asset.Parent.Name
                                .Replace( "_us.dat", ".dtt" );
                            }
                            Debug.Assert( File.Exists( dataStorageName ) );

                            data.LoadExternalDataStorage( dataStorageName );
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }

                    success = true;
                }
                catch ( Exception )
                {
                }

                if ( success )
                {
                    asset.Data = data;

                    foreach ( var subFile in data.Textures )
                    {
                        foreach ( var item in EnumerateAssetsInternal( options,
                            new AssetInfo
                            {
                                Name = "",
                                Stream = subFile.GetStream(),
                                Parent = asset,
                                Tag = subFile,
                            } ) )
                        {
                            yield return item;
                        }
                    }
                }
            }
        }
    }

    foreach ( var file in Directory.EnumerateFiles( options.Path, "*.*", SearchOption.AllDirectories ) )
    {
        var stream = File.OpenRead( file );
        foreach ( var item in EnumerateAssetsInternal( options, new AssetInfo { Name = file, Stream = stream, Parent = null } ) )
            yield return item;
    }
}

static void DumpGroupInfo( Model data, IEnumerable<AssetInfo> assets )
{
    if ( data.Groups.Count == 0 )
        return;

    var maxLodMeshId4 = -1;
    var noOfMaxLodMesh4 = 0;


    foreach ( var item in data.Groups )
    {
        noOfMaxLodMesh4 += item.LodMeshIndices.Length;
        foreach ( var item2 in item.LodMeshIndices[ 4 ] )
        {
            Debug.Assert( item2 < data.Materials.Count );
            //Debug.Assert( item2 < data.Groups.Count );
            //Debug.Assert( item2 < data.MatrixPalettes.Count );
            //Debug.Assert( item2 < data.Meshes.Length );
            //Debug.Assert( item2 < data.Meshes[0].Count );
            //Debug.Assert( item2 < data.Meshes[ 1 ].Count );
            //Debug.Assert( item2 < data.Meshes[ 2 ].Count );
            //Debug.Assert( item2 < data.Meshes[ 3 ].Count );
            //Debug.Assert( item2 < data.Nodes.Count );
            //Debug.Assert( item2 < data.SubMeshes.Count );
            //Debug.Assert( item2 < data.Textures.Count );
            maxLodMeshId4 = Math.Max( maxLodMeshId4, item2 );
        }
    }

    Console.WriteLine( $"Max lod mesh id (4) = {maxLodMeshId4} (mesh # {data.Meshes[ 0 ].Count}, {data.Meshes[ 1 ].Count}, {data.Meshes[ 2 ].Count}, {data.Meshes[ 3 ].Count})" );
    Console.WriteLine( $"Number of lod mesh id 4 entries: {noOfMaxLodMesh4}" );
}

static void DumpTextureInfo( Options options, IEnumerable<AssetInfo> assets )
{
    var usedValues = new Dictionary<string, HashSet<AssetValue>>();
    void Add( string name, AssetInfo asset, dynamic value )
    {
        if ( !usedValues.ContainsKey( name ) )
            usedValues.Add( name, new HashSet<AssetValue>() );
        usedValues[ name ].Add( new AssetValue( asset, value ) );
    }

    foreach ( var asset in assets )
    {
        if ( asset.Stream.Length == 0 || !(asset.Name.EndsWith( ".wtb" ) || asset.Name.EndsWith(".wta")) )
            continue;

        try
        {
            var data = new WtbArchive( asset.Stream, true );
            foreach ( var tex in data.Textures )
            {
                Add( "archive texture flags", asset, tex.Flags.ToString("X8") );
                Add( "archive texture hash", asset, tex.Hash.ToString( "X8" ) );
            }
        }
        catch ( InvalidFileFormatException ex )
        {
            Console.WriteLine( $"{asset.Parent?.Name} {asset.Name} failed to load" );
        }
    }

    foreach ( (var key, var value) in usedValues )
    {
        Console.WriteLine( key );
        foreach ( var item in value.OrderBy( x => x ) )
            Console.WriteLine( "\t" + item );
        Console.WriteLine();
    }
}

static void DumpModelTextureInfo( Options options, IEnumerable<AssetInfo> assets )
{
    var usedValues = new Dictionary<string, HashSet<dynamic>>();
    void Add(string name, dynamic value )
    {
        if (!usedValues.ContainsKey(name))
            usedValues.Add(name, new HashSet<dynamic>());
        usedValues[name].Add(value);
    }

    foreach ( var asset in assets )
    {
        if ( !asset.Name.EndsWith( ".wmb" ) )
            continue;

        try
        {
            var model = new Model( asset.Stream, true );
            foreach ( var tex in model.Textures )
            {
                Add( "model texture type", tex.Type );
                Add( "model texture hash", tex.Hash.ToString("X8") );
            }
        }
        catch ( InvalidFileFormatException ex )
        {
            Console.WriteLine( $"{asset.Parent?.Name} {asset.Name} failed to load" );
        }
    }

    foreach ( (var key, var value ) in usedValues )
    {
        Console.WriteLine( key );
        foreach ( var item in value.OrderBy( x => x ) )
            Console.WriteLine( "\t" + item );
        Console.WriteLine();
    }
}

static void DumpMaterialInfo( Options options, IEnumerable<AssetInfo> assets )
{
    var usedShaderNames = new HashSet<string>();
    var usedField08 = new HashSet<int>();
    var usedField10 = new HashSet<int>();
    var usedField12 = new HashSet<int>();
    var usedField14 = new HashSet<int>();
    var textureCountByShader = new Dictionary<string, int>();
    var field10ByShader = new Dictionary<string, int>();
    var field12ByShader = new Dictionary<string, int>();

    foreach ( var asset in assets  )
    {
        if ( !asset.Name.EndsWith( ".wmb" ) )
            continue;

        try
        {
            var model = new Model( asset.Stream, true );
            foreach ( var mat in model.Materials )
            {
                usedField08.Add( mat.Field08 );
                usedField10.Add( mat.Field10 );
                usedField12.Add( mat.Field12 );
                usedField14.Add( mat.Field14 );
                usedShaderNames.Add( mat.ShaderName );

                if ( !textureCountByShader.ContainsKey( mat.ShaderName ) ) textureCountByShader[ mat.ShaderName ] = mat.TextureIndices.Count;
                else Debug.Assert( mat.TextureIndices.Count == textureCountByShader[ mat.ShaderName ] );


                //if ( !field10ByShader.ContainsKey( mat.ShaderName ) ) field10ByShader[ mat.ShaderName ] = mat.Field10;
                //else Debug.Assert( mat.Field10 == field10ByShader[ mat.ShaderName ] );

                if ( !field12ByShader.ContainsKey( mat.ShaderName ) ) field12ByShader[ mat.ShaderName ] = mat.Field12;
                else Debug.Assert( mat.Field12 == field12ByShader[ mat.ShaderName ] );

                //Debug.Assert( mat.Field12 <= mat.TextureIndices.Count && mat.TextureIndices.Skip( mat.Field12 ).All( x => x == 0 ) );

                //Debug.Assert( mat.TextureIndices.Count == mat.Field08 );
                //Debug.Assert( mat.TextureIndices.Count == mat.Field10 );
                //Debug.Assert( mat.TextureIndices.Count == mat.Field12 );
                //Debug.Assert( mat.TextureIndices.Count == mat.Field14 );
            }
        }
        catch ( InvalidFileFormatException ex )
        {
            Console.WriteLine( $"{asset.Parent?.Name} {asset.Name} failed to load" );
        }
    }

    Console.WriteLine( "usedField08" );
    foreach ( var item in usedField08.OrderBy( x => x ) )
        Console.WriteLine( item );
    Console.WriteLine();

    Console.WriteLine( "usedField10" );
    foreach ( var item in usedField10.OrderBy( x => x ) )
        Console.WriteLine( item );
    Console.WriteLine();

    Console.WriteLine( "usedField12" );
    foreach ( var item in usedField12.OrderBy( x => x ) )
        Console.WriteLine( item );
    Console.WriteLine();

    Console.WriteLine( "usedField14" );
    foreach ( var item in usedField14.OrderBy( x => x ) )
        Console.WriteLine( item );
    Console.WriteLine();

    Console.WriteLine( "field12ByShader" );
    foreach ( (var key, var value) in field12ByShader )
        Console.WriteLine( $"{key} {value}" );
    Console.WriteLine();

    Console.WriteLine( "textureCountByShader" );
    foreach ( (var key, var value) in textureCountByShader )
        Console.WriteLine( $"{key} {value}" );
    Console.WriteLine();

    Console.WriteLine( "usedShaderNames" );
    foreach ( var item in usedShaderNames.OrderBy( x => x ) )
        Console.WriteLine( item );
    Console.WriteLine();
}

class AssetInfo
{
    private object data;

    public string Name { get; set; }
    public Stream Stream { get; set; }
    public AssetInfo Parent { get; set; }
    public object Tag { get; set; }
    public object Data
    {
        get
        {
            if ( data == null )
            {
                try
                {
                    var ext = Path.GetExtension( Name ).ToLower();
                    if ( ext == ".wmb" )
                    {
                        data = new Model( Stream, true );
                    }
                    else if ( ext == ".dat" || ext == ".dtt" || ext == ".evn" )
                    {
                        data = new DatArchive( Stream );
                    }
                    else if ( ext == ".wtb" || ext == ".wta" )
                    {
                        data = new WtbArchive( Stream );
                    }
                    else if ( ext == ".bxm" )
                    {
                        // TODO
                    }
                }
                catch ( Exception )
                {
                }

            }

            return data;
        }

        set
        {
            data = value;
        }
    }

    public override string ToString()
    {
        if ( Parent != null )
            return $"{Parent}/{Name}";
        else
            return Name;
    }


}

class AssetValue : IEquatable<AssetValue>, IComparable<AssetValue>, IComparable
{
    public AssetInfo Asset { get; set; }
    public dynamic Value { get; set; }

    public AssetValue( AssetInfo asset, dynamic value )
    {
        Asset = asset;
        Value = value;
    }

    public bool Equals( AssetValue other )
    {
        return Value.Equals( other.Value );
    }

    public int CompareTo( AssetValue other )
    {
        return Value.CompareTo( other.Value );
    }

    public override string ToString()
    {
        return $"{Value} ({Asset})";
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public int CompareTo( object? obj )
    {
        var other = obj as AssetValue;
        if ( other == null ) return 0;
        return CompareTo( other );
    }
}

class Options
{
    [Option("path", Required = true)]
    public string Path { get; set; }

    [Option("extract-files", Default = false, Required = false)]
    public bool ExtractFiles { get; set; }

    [Option( "dump-model-info", Default = false, Required = false )]
    public bool DumpModelInfo { get; set; }

    [Option( "dump-texture-info", Default = false, Required = false )]
    public bool DumpTextureInfo { get; set; }

    [Option( "dump-model-texture-info", Default = false, Required = false )]
    public bool DumpModelTextureInfo { get; set; }

    [Option( "dump-material-info", Default = false, Required = false )]
    public bool DumpMaterialInfo { get; set; }
}