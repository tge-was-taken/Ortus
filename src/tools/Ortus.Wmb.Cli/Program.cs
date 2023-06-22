using Ortus.Models.Converters.Assimp;
using Ortus.Models.Wmb4;

ModelHelper.ReplaceModel(
    new AssimpModelConverter(),
    @"X:\project\mgr\dumps\data000.cpk\pl\pl0100.dat",
    @"D:\game\PC\SteamLibrary\steamapps\common\METAL GEAR RISING REVENGEANCE\GameData\pl\pl0100.dat",
    @"X:\project\mgr\dumps\test.fbx" );