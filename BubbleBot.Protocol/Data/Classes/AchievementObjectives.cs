using Newtonsoft.Json;

namespace BubbleBot.Protocol.Data
{
    public class AchievementObjectives : IData
    {

        // Properties
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("achievementId")]
        public int AchievementId { get; set; }
        [JsonProperty("nameId")]
        public string NameId { get; set; }
        [JsonProperty("criterion")]
        public string Criterion { get; set; }


        //Constructor
        internal AchievementObjectives() { }

    }
}
