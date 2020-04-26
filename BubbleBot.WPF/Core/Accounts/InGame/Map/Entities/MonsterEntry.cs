using BubbleBot.Data;
using BubbleBot.Protocol.Data;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Core.Accounts.InGame.Map.Entities
{
    public class MonsterEntry
    {
        // Constructor
        public MonsterEntry(MonsterInGroupLightInformations infos)
        {
            GenericId = infos.CreatureGenericId;
            Grade = (byte) infos.Grade;

            var m = DataManager.Get<Monsters>(GenericId);
            Name = m.NameId;
            Level = m?.Grades[Grade - 1].level;
            Boss = m.IsBoss;
            MiniBoss = m.IsMiniBoss;
            QuestMonster = m.IsQuestMonster;
        }

        // Properties
        public int GenericId { get; }
        public byte Grade { get; }
        public string Name { get; }
        public int Level { get; }
        public bool Boss { get; }
        public bool MiniBoss { get; }
        public bool QuestMonster { get; }
    }
}