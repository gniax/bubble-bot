using BubbleBot.Protocol.Data;
using BubbleBot.Protocol.Messages;
using BubbleBot.Protocol.Types;
using BubbleBot.Utility.DofusTouch;
using GalaSoft.MvvmLight;
using BubbleBot.Data;

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
            SetMinPlayerLevel(spell);
        }

        public SpellEntry(int spellId, uint level)
        {
            var spell = DataManager.Get<Spells>(spellId);

            Id = spellId;
            Level = (byte) level;
            IconId = spell.IconId;
            Name = spell.NameId;
            SetMinPlayerLevel(spell);
        }

        // Properties
        public int Id { get; }
        public byte Level { get; private set; }
        public string Name { get; }
        public int MinPlayerLevel { get; private set; }
        public int IconId { get; }

        public string IconUrl =>
            $"https://dofustouch.cdn.ankama.com/assets/{DTConstants.AssetsVersion}/gfx/spells/sort_{IconId}.png";


        #region Updates

        public void Update(SpellUpgradeSuccessMessage message)
        {
            Level = (byte) message.SpellLevel;
            RaisePropertyChanged("Level");
        }

        #endregion

        private void SetMinPlayerLevel(Spells spell)
        {
            var spelllevel = DataManager.Get<SpellLevels>(spell.SpellLevels[Level - 1]);
            MinPlayerLevel = spelllevel.MinPlayerLevel;
        }
    }
}