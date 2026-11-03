using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CharacterNicknameMod.Tests
{
    public sealed class CharacterNicknameConfigTests
    {
        [Fact]
        public void ManifestJson_IsValid()
        {
            string manifestPath = Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..", "..",
                "CharacterNicknameMod.json"));

            Assert.True(File.Exists(manifestPath), "Manifest file should exist at: " + manifestPath);
            string json = File.ReadAllText(manifestPath);
            var obj = JObject.Parse(json);

            Assert.Equal("CharacterNicknameMod", (string)obj["id"]);
            Assert.Equal("隐藏队友昵称", (string)obj["name"]);
            Assert.False(string.IsNullOrWhiteSpace((string)obj["version"]));
        }

        [Fact]
        public void ConfigJson_ContainsAllOptions_AndFullI18nComments()
        {
            string configPath = Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..", "..",
                "config.json"));

            Assert.True(File.Exists(configPath), "config.json should exist at: " + configPath);
            string json = File.ReadAllText(configPath);
            var obj = JObject.Parse(json);

            Assert.True(obj.ContainsKey("enabled"), "config.json must have enabled");
            Assert.True(obj.ContainsKey("anonymizeSelf"), "config.json must have anonymizeSelf");
            Assert.False((bool)obj["anonymizeSelf"], "anonymizeSelf must be false by default");
            Assert.True(obj.ContainsKey("changeAvatarNickname"), "config.json must have changeAvatarNickname");
            Assert.True(obj.ContainsKey("changeThinkingBanner"), "config.json must have changeThinkingBanner");

            var comments = obj["_comments"] as JObject;
            Assert.NotNull(comments);

            string[] keys = { "enabled", "anonymizeSelf", "changeAvatarNickname", "changeThinkingBanner" };
            string[] langs = { "zh-Hans", "zh-Hant", "en", "ja" };

            foreach (var key in keys)
            {
                Assert.True(comments.ContainsKey(key), $"_comments must contain '{key}'");
                foreach (var lang in langs)
                {
                    var text = comments[key]?[lang]?.ToString();
                    Assert.False(string.IsNullOrWhiteSpace(text), $"_comments.{key} must have non-empty {lang}");
                }
            }
        }

        [Fact]
        public void Settings_DefaultValues_AreCorrect()
        {
            var settings = new CharacterNicknameSettings();
            Assert.True(settings.Enabled);
            Assert.False(settings.AnonymizeSelf);
            Assert.True(settings.ChangeAvatarNickname);
            Assert.True(settings.ChangeThinkingBanner);
        }
    }
}
