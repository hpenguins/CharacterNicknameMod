using System;
using System.Collections.Generic;
using System.Reflection;
using FairyGUI;
using GameLogic;
using Tools;
using UI;

namespace CharacterNicknameMod
{
    internal sealed class NicknameManager
    {
        private static readonly FieldInfo NickField = typeof(RoomPlayer).GetField("_nick", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo CoverNameIdField = typeof(RoomPlayer).GetField("_coverNameId", BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly Dictionary<long, string> _originalNicks = new Dictionary<long, string>();
        private readonly Dictionary<long, string> _appliedNames = new Dictionary<long, string>();

        public int AppliedCount => _appliedNames.Count;

        public void Update(CharacterNicknameSettings settings)
        {
            if (!settings.Enabled) return;

            UpdateDataLayer(settings);

            if (settings.ChangeAvatarNickname)
            {
                UpdateAvatarNicknames();
            }

            if (settings.ChangeThinkingBanner)
            {
                UpdateThinkingBanner();
            }
        }

        private void UpdateDataLayer(CharacterNicknameSettings settings)
        {
            var gm = SimpleSingletonProvider<GameLogicManager>.inst;
            if (gm == null || gm.battle == null || gm.battle.PlayerDatas == null) return;

            var list = gm.battle.PlayerDatas;
            for (int i = 0; i < list.Count; i++)
            {
                var pd = list[i];
                if (pd == null || pd.player == null) continue;
                long id = pd.player.Id;

                bool isSelf = IsSelf(id);
                if (isSelf && !settings.AnonymizeSelf)
                {
                    // 若先前被包含过但现在 anonymizeSelf=false，则还原自己
                    string orig;
                    if (_originalNicks.TryGetValue(id, out orig) && !string.IsNullOrEmpty(orig))
                    {
                        ApplyPlayerNick(pd.player, orig, recordOriginal: false);
                        _appliedNames.Remove(id);
                    }
                    continue;
                }

                string charName = GetCharacterDisplayName(pd);
                if (string.IsNullOrEmpty(charName)) continue;

                ApplyPlayerNick(pd.player, charName, recordOriginal: true);
            }
        }

        private void ApplyPlayerNick(RoomPlayer player, string newNick, bool recordOriginal)
        {
            if (player == null || string.IsNullOrEmpty(newNick)) return;
            long id = player.Id;

            if (recordOriginal && !_originalNicks.ContainsKey(id))
            {
                string raw = player.serverPlayer != null && !string.IsNullOrEmpty(player.serverPlayer.Nick)
                    ? player.serverPlayer.Nick
                    : (NickField != null ? NickField.GetValue(player) as string : null);
                _originalNicks[id] = raw ?? "";
            }

            if (recordOriginal)
            {
                _appliedNames[id] = newNick;
            }

            try
            {
                if (NickField != null) NickField.SetValue(player, newNick);
            }
            catch { }

            try
            {
                if (player.serverPlayer != null) player.serverPlayer.Nick = newNick;
            }
            catch { }

            try
            {
                if (CoverNameIdField != null) CoverNameIdField.SetValue(player, 0);
            }
            catch { }
        }

        private void UpdateAvatarNicknames()
        {
            UIBattleInfoPanel panel = FindFirst<UIBattleInfoPanel>(GRoot.inst);
            if (panel == null || panel.com_BattlePlayer == null) return;
            var container = panel.com_BattlePlayer.com_Container;
            if (container == null) return;

            var slots = new[]
            {
                container.com_Player_1,
                container.com_Player_2,
                container.com_Player_3,
                container.com_Player_4
            };

            for (int i = 0; i < slots.Length; i++)
            {
                var slot = slots[i];
                if (slot == null || slot.PlayerData == null || slot.PlayerData.player == null) continue;
                long id = slot.PlayerData.player.Id;

                string targetName;
                if (!_appliedNames.TryGetValue(id, out targetName) || string.IsNullOrEmpty(targetName))
                    continue;

                var info = slot.com_player as UICom_PlayerInfo;
                if (info != null && info.txt_PlayerName != null)
                {
                    if (info.txt_PlayerName.text != targetName)
                    {
                        info.txt_PlayerName.text = targetName;
                    }
                }
            }
        }

        private void UpdateThinkingBanner()
        {
            try
            {
                var ui = SimpleSingletonProvider<UIManager>.inst;
                if (ui == null || ui.tips == null || !ui.tips.isShowing) return;

                var win = ui.tips.contentPane as UITipsWindow;
                if (win == null || win.com_Thinking == null || !win.com_Thinking.visible) return;
                if (win.com_Thinking.txt_Title == null || string.IsNullOrEmpty(win.com_Thinking.txt_Title.text)) return;

                string current = win.com_Thinking.txt_Title.text;
                string updated = current;

                foreach (var pair in _originalNicks)
                {
                    string orig = pair.Value;
                    if (string.IsNullOrEmpty(orig)) continue;

                    string charName;
                    if (_appliedNames.TryGetValue(pair.Key, out charName) && !string.IsNullOrEmpty(charName))
                    {
                        if (updated.Contains(orig))
                        {
                            updated = updated.Replace(orig, charName);
                        }
                    }
                }

                if (updated != current)
                {
                    win.com_Thinking.txt_Title.text = updated;
                }
            }
            catch { }
        }

        public void Restore()
        {
            try
            {
                var gm = SimpleSingletonProvider<GameLogicManager>.inst;
                if (gm != null && gm.battle != null && gm.battle.PlayerDatas != null)
                {
                    foreach (var pd in gm.battle.PlayerDatas)
                    {
                        if (pd == null || pd.player == null) continue;
                        long id = pd.player.Id;
                        string orig;
                        if (_originalNicks.TryGetValue(id, out orig) && !string.IsNullOrEmpty(orig))
                        {
                            try { if (NickField != null) NickField.SetValue(pd.player, orig); } catch { }
                            try { if (pd.player.serverPlayer != null) pd.player.serverPlayer.Nick = orig; } catch { }
                        }
                    }
                }

                UIBattleInfoPanel panel = FindFirst<UIBattleInfoPanel>(GRoot.inst);
                if (panel != null && panel.com_BattlePlayer != null && panel.com_BattlePlayer.com_Container != null)
                {
                    var container = panel.com_BattlePlayer.com_Container;
                    var slots = new[]
                    {
                        container.com_Player_1,
                        container.com_Player_2,
                        container.com_Player_3,
                        container.com_Player_4
                    };
                    for (int i = 0; i < slots.Length; i++)
                    {
                        var slot = slots[i];
                        if (slot != null && slot.com_player is UICom_PlayerInfo info && slot.PlayerData != null && slot.PlayerData.player != null)
                        {
                            string orig;
                            if (_originalNicks.TryGetValue(slot.PlayerData.player.Id, out orig) && !string.IsNullOrEmpty(orig))
                            {
                                info.txt_PlayerName.text = orig;
                            }
                        }
                    }
                }
            }
            catch { }

            _originalNicks.Clear();
            _appliedNames.Clear();
        }

        public static string GetCharacterDisplayName(BattlePlayerData data)
        {
            if (data == null || data.player == null) return null;
            try
            {
                if (data.player.Hero != null && data.player.Hero.HeroId > 0)
                {
                    int heroId = data.player.Hero.HeroId;
                    // CharacterHandle.GetCharacterName 取 NameID（如茉莉），而不是 GetCharacterNickName（家政机器人）
                    string name = CharacterHandle.GetCharacterName(heroId, CharacterType.Hero);
                    if (!string.IsNullOrEmpty(name)) return name;
                }

                if (data.player.characterConfig != null && data.player.characterConfig.Id > 0)
                {
                    string name = CharacterHandle.GetCharacterName(data.player.characterConfig.Id, data.characterType);
                    if (!string.IsNullOrEmpty(name)) return name;
                }
            }
            catch { }
            return null;
        }

        private static bool IsSelf(long playerId)
        {
            try
            {
                var gm = SimpleSingletonProvider<GameLogicManager>.inst;
                if (gm != null)
                {
                    if (gm.account != null && gm.account.IsSelf(playerId)) return true;
                    var self = gm.battle != null ? gm.battle.GetSelfPlayerData() : null;
                    if (self != null && self.player != null && self.player.Id == playerId) return true;
                }
            }
            catch { }
            return false;
        }

        private static T FindFirst<T>(GObject node, int depth = 0) where T : GObject
        {
            if (node == null || depth > 20) return null;
            try { if (node.isDisposed) return null; } catch { return null; }

            T hit = node as T;
            if (hit != null) return hit;

            GComponent component = node as GComponent;
            if (component == null) return null;
            for (int i = 0; i < component.numChildren; i++)
            {
                T found = FindFirst<T>(component.GetChildAt(i), depth + 1);
                if (found != null) return found;
            }
            return null;
        }
    }
}
