using CesiumLoader.SDK.Configuration;

namespace CharacterNicknameMod
{
    /// <summary>config.json 里各字段的多语言说明。</summary>
    internal static class CharacterNicknameConfigDocs
    {
        internal static bool Apply(ModConfig config)
        {
            if (config == null) return false;
            bool changed = false;

            changed |= config.Describe("enabled", D(
                "总开关：开启后生效。",
                "總開關：開啟後生效。",
                "Master switch: activates the mod when true.",
                "全体スイッチ：有効にすると Mod が動作します。"));

            changed |= config.Describe("anonymizeSelf", D(
                "是否同时匿名自己的昵称，默认关闭（仅匿名队友）。",
                "是否同時匿名自己的暱稱，預設關閉（僅匿名隊友）。",
                "Whether to also anonymize your own nickname (off by default, teammates only).",
                "自分の名前も匿名化（キャラクター名に変更）するかどうか（既定オフ、チームメイトのみ）。"));

            changed |= config.Describe("changeAvatarNickname", D(
                "是否修改左侧头像旁的昵称文本。",
                "是否修改左側頭像旁的暱稱文字。",
                "Whether to update nickname labels next to player avatars.",
                "アバターの横にあるプレイヤー名を変更するかどうか。"));

            changed |= config.Describe("changeThinkingBanner", D(
                "是否修改选择筹码等思考横幅中的昵称文本。",
                "是否修改選擇籌碼等思考橫幅中的暱稱文字。",
                "Whether to update nicknames shown in thinking banners (e.g. relic selection).",
                "レリック選択などの思考中バナーに表示されるプレイヤー名を変更するかどうか。"));

            return changed;
        }

        private static ConfigDoc D(string zhHans, string zhHant, string en, string ja)
            => ConfigDoc.Of(zhHans, zhHant, en, ja);
    }
}
