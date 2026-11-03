using System;
using CesiumLoader.SDK.Configuration;

namespace CharacterNicknameMod
{
    internal sealed class CharacterNicknameSettings
    {
        public bool Enabled = true;
        /// <summary>是否同时匿名自己的昵称，默认 false（仅匿名队友）。</summary>
        public bool AnonymizeSelf;
        public bool ChangeAvatarNickname = true;
        public bool ChangeThinkingBanner = true;

        public static CharacterNicknameSettings Read(ModConfig config, Action<string> warn)
        {
            var value = new CharacterNicknameSettings();
            if (config == null) return value;

            value.Enabled = config.GetBool("enabled", true);
            value.AnonymizeSelf = config.GetBool("anonymizeSelf", false);
            value.ChangeAvatarNickname = config.GetBool("changeAvatarNickname", true);
            value.ChangeThinkingBanner = config.GetBool("changeThinkingBanner", true);
            return value;
        }

        public static void EnsureDefaults(ModConfig config)
        {
            if (config == null) return;

            bool added = CharacterNicknameConfigDocs.Apply(config);
            Add(config, "enabled", true, ref added);
            Add(config, "anonymizeSelf", false, ref added);
            Add(config, "changeAvatarNickname", true, ref added);
            Add(config, "changeThinkingBanner", true, ref added);
            if (added) config.SaveIfDirty();
        }

        private static void Add(ModConfig config, string key, object value, ref bool added)
        {
            if (config.Has(key)) return;
            config.Set(key, value);
            added = true;
        }
    }
}
