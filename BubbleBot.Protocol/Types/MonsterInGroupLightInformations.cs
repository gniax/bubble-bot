using Newtonsoft.Json;

namespace BubbleBot.Protocol.Types
{
    public class MonsterInGroupLightInformations
    {

        // Properties
        public int CreatureGenericId { get; set; }
        public uint Grade { get; set; }

        [JsonProperty("staticInfos")]
        public MainCreatureLightInfosStaticInfos StaticInfos { get; set; }


        // Constructors
        public MonsterInGroupLightInformations() { }

        public MonsterInGroupLightInformations(int creatureGenericId = 0, uint grade = 0, MainCreatureLightInfosStaticInfos staticInfos = null)
        {
            CreatureGenericId = creatureGenericId;
            Grade = grade;
            StaticInfos = staticInfos;
        }

    }
}
