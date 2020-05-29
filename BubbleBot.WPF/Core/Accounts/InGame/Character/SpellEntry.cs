using BubbleBot.Protocol.Data;
using BubbleBot.Protocol.Messages;
using BubbleBot.Protocol.Types;
using BubbleBot.Utility.DofusTouch;
using GalaSoft.MvvmLight;
using BubbleBot.Data;
using System.Threading.Tasks;
using System;

namespace BubbleBot.Core.Accounts.InGame.Character
{
    public class SpellEntry : ViewModelBase
    {
        // Constructors
        public SpellEntry(SpellItem s, Spells spell)
        {
            Id = s.SpellId;
            Level = (byte) s.SpellLevel;
            Name = spell.NameId;
            IconId = spell.IconId;
            SetMinPlayerLevelAsync(spell);
        }

        public SpellEntry(int spellId, uint level)
        {
            SetSpellEntryInformations( spellId, level);
        }

        // Properties
        public int Id { get; private set; }
        public byte Level { get; private set; }
        public string Name { get; private set; }
        public int MinPlayerLevel { get; private set; }
        public int IconId { get; private set; }

        public string IconUrl =>
            $"https://dofustouch.cdn.ankama.com/assets/{DTConstants.AssetsVersion}/gfx/spells/sort_{IconId}.png";

        private async void SetSpellEntryInformations(int spellId, uint level)
        {
            var spell = await DataManager.Get<Spells>(spellId);

            Id = spellId;
            Level = (byte)level;
            IconId = spell.IconId;
            Name = spell.NameId;
            SetMinPlayerLevelAsync(spell);
        }
        private async void SetMinPlayerLevelAsync(Spells spell)
        {
            if (Level > 0)
            {
                var spelllevel = await DataManager.Get<SpellLevels>(spell.SpellLevels[Level - 1]);
                if (spelllevel != null)
                    MinPlayerLevel = spelllevel.MinPlayerLevel;
            }
        }

        #region Updates

        public void Update(SpellUpgradeSuccessMessage message)
        {
            Level = (byte) message.SpellLevel;
            RaisePropertyChanged("Level");
        }

        #endregion

    }
}