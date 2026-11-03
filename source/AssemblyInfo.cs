using CesiumLoader.SDK.Manifests;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CharacterNicknameMod.Tests")]

[assembly: ModManifest("角色昵称", "1.0.0", "hpenguins",
    "将队友昵称显示为当前游玩角色的名称（例如茉莉），并同步修改头像旁昵称与选筹码等思考横幅。",
    Permissions = ModPermission.ReadGameState | ModPermission.Input | ModPermission.UI,
    SdkVersion = "2.8.6")]
