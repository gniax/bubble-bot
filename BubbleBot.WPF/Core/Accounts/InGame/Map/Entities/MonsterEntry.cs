using BubbleBot.Data;
using BubbleBot.Protocol.Data;
using BubbleBot.Protocol.Types;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.InGame.Map.Entities
{
    public class MonsterEntry
    {
        // Constructor
        public MonsterEntry(MonsterInGroupLightInformations infos)
        {
            GenericId = infos.CreatureGenericId;
            Grade = (byte) infos.Grade;
            SetMonsterInformations();


        }

        // Properties
        public int GenericId { get;}
        public byte Grade { get;}
        public string Name { get; set; }
        public int Level { get; set; }
        public bool Boss { get; set; }
        public bool MiniBoss { get; set; }
        public bool QuestMonster { get; set; }

        private async void SetMonsterInformations()
        {
            var m = await DataManager.Get<Monsters>(GenericId);
            Name = m.NameId;
            Level = m?.Grades[Grade - 1].level;
            Boss = m.IsBoss;
            MiniBoss = m.IsMiniBoss;
            QuestMonster = m.IsQuestMonster;
        }
    }
}