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

            var m = DataManager.Get<Monsters>(CreatureGenericId);
            Name = m.NameId;
            IsBoss = m.IsBoss;
            IsMiniBoss = m.IsMiniBoss;
            IsQuestMonster = m.IsQuestMonster;
            Level = (short) m.Grades[(int) infos1.CreatureGrade - 1].level;
        }

        // Properties
        public int CreatureGenericId { get; }
        public string Name { get; }
        public short Level { get; }
        public bool IsBoss { get; }
        public bool IsMiniBoss { get; }
        public bool IsQuestMonster { get; }
    }
}