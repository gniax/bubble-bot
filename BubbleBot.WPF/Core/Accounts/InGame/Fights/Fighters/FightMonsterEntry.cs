using BubbleBot.Protocol.Data;
using BubbleBot.Protocol.Types;
using BubbleBot.Data;

namespace BubbleBot.Core.Accounts.InGame.Fights.Fighters
{
    public class FightMonsterEntry : FighterEntry
    {
        // Constructor
        public FightMonsterEntry(GameFightMonsterInformations infos1, GameFightFighterInformations infos2) :
            base(infos2)
        {
            CreatureGenericId = (int) infos1.CreatureGenericId;
            SetMonsterEntry(infos1);

        }

        // Properties
        public int CreatureGenericId { get; }
        public string Name { get; set; }
        public short Level { get; set; }
        public bool IsBoss { get; set; }
        public bool IsMiniBoss { get; set; }
        public bool IsQuestMonster { get; set; }

        private async void SetMonsterEntry(GameFightMonsterInformations infos1)
        {
            var m = await DataManager.Get<Monsters>(CreatureGenericId);
            Name = m.NameId;
            IsBoss = m.IsBoss;
            IsMiniBoss = m.IsMiniBoss;
            IsQuestMonster = m.IsQuestMonster;
            Level = (short)m.Grades[(int)infos1.CreatureGrade - 1].level;
        }

    }
}