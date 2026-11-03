# 角色昵称 CharacterNicknameMod

在战斗中将队友（或自己）的游戏昵称显示为当前所使用角色的名称（例如别人玩茉莉时显示为“茉莉”，而不是其账号昵称或家政机器人称号）。

同步修改左侧玩家头像旁昵称、选择筹码等思考横幅中的昵称。

## 功能特性

- **角色名称替换**：自动读取角色配置的正式名称（`NameID` 本地化文本），随当前游戏语言自动切换（如简中“茉莉”、日文“モリー”、英文“Molly”）。
- **头像旁昵称**：实时同步左侧玩家头像栏中的玩家昵称文本。
- **思考横幅同步**：当队友在选择筹码或思考行动时，屏幕居中横幅显示的名称同步使用角色名。
- **自身匿名开关**：支持仅修改队友昵称（默认），或开启 `anonymizeSelf` 同时把自己的昵称也改成所用角色名。
- **纯客户端显示**：仅修改本地 UI 与显示数据层，不发送任何异常网络请求，不影响对局数据与结算。

## 配置

配置文件路径：`AstralParty_ModLoader\mods\CharacterNicknameMod\config.json`。首次运行会自动生成。

```json
{
  "enabled": true,
  "anonymizeSelf": false,
  "changeAvatarNickname": true,
  "changeThinkingBanner": true
}
```

| 字段 | 默认值 | 说明 |
| --- | --- | --- |
| `enabled` | `true` | 总开关，关闭后恢复原始昵称 |
| `anonymizeSelf` | `false` | 是否同时把自己的昵称也改成角色名（默认仅匿名队友） |
| `changeAvatarNickname` | `true` | 是否修改左侧玩家头像旁的昵称文本 |
| `changeThinkingBanner` | `true` | 是否修改选择筹码等思考横幅中的玩家名称 |

也可以在 Toys 的「模组管理」中直接可视化修改配置。

## 安装方法

将编译产出的 `CharacterNicknameMod.dll`、`CharacterNicknameMod.json` 及 `config.json` 放入游戏目录的 `AstralParty_ModLoader\mods\CharacterNicknameMod\` 文件夹中即可。
