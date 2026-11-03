using System;
using CesiumLoader.SDK.Engine;
using CesiumLoader.SDK.Mods;
using UnityEngine.SceneManagement;

namespace CharacterNicknameMod
{
    public static class ModEntry
    {
        public static void Main()
        {
            ModBase.Run(new CharacterNicknameController(), tag: "CharNick");
        }
    }

    public sealed class CharacterNicknameController : ModBase
    {
        private CharacterNicknameSettings _settings = new CharacterNicknameSettings();
        private readonly NicknameManager _manager = new NicknameManager();

        private float _nextScan;
        private float _nextErrorLog;
        private string _lastError;

        public override string Name => "角色昵称";
        public override string Version => "1.0.0";
        public override string Author => "hpenguins";
        public override string Description => "将队友昵称显示为当前游玩角色的名称（例如茉莉），并同步修改头像旁昵称与选筹码等思考横幅。";

        public override void OnInitialize()
        {
            CharacterNicknameSettings.EnsureDefaults(Config);
            _settings = CharacterNicknameSettings.Read(Config, warning => Log.Warn(warning));

            Log.Info("角色昵称: " + DescribeSettings());
            if (!_settings.Enabled)
            {
                Log.Info("角色昵称处于禁用状态（enabled=false）");
            }
        }

        public override void OnConfigChanged()
        {
            bool wasEnabled = _settings.Enabled;
            _settings = CharacterNicknameSettings.Read(Config, warning => Log.Warn(warning));

            if (wasEnabled && !_settings.Enabled)
            {
                _manager.Restore();
                Log.Info("角色昵称已禁用，已恢复原始昵称");
            }
            else
            {
                Log.Info("角色昵称配置已更新: " + DescribeSettings());
            }
        }

        public override void OnUpdate()
        {
            if (!_settings.Enabled) return;

            try
            {
                float now = UnityTime.RealtimeSinceStartup;
                if (now >= _nextScan)
                {
                    _nextScan = now + 0.25f;
                    _manager.Update(_settings);
                }
            }
            catch (Exception e)
            {
                float now = UnityTime.RealtimeSinceStartup;
                string msg = e.GetType().Name + ": " + e.Message;
                if (msg != _lastError && now >= _nextErrorLog)
                {
                    _lastError = msg;
                    _nextErrorLog = now + 15f;
                    Log.Warn("更新角色昵称失败: " + msg);
                }
            }
        }

        public override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _manager.Restore();
        }

        public override void OnUnload()
        {
            _manager.Restore();
            Log.Info("角色昵称已卸载，已恢复原始昵称");
        }

        private string DescribeSettings()
        {
            return "enabled=" + _settings.Enabled
                + ", anonymizeSelf=" + _settings.AnonymizeSelf
                + ", changeAvatar=" + _settings.ChangeAvatarNickname
                + ", changeBanner=" + _settings.ChangeThinkingBanner;
        }
    }
}
