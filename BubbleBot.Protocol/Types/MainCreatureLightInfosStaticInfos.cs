using Newtonsoft.Json;

namespace BubbleBot.Protocol.Types
{
    public partial class MainCreatureLightInfosStaticInfos
    {
        [JsonProperty("nameId")]
        public string NameId { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("isMiniBoss")]
        public bool IsMiniBoss { get; set; }

        [JsonProperty("isBoss")]
        public bool IsBoss { get; set; }

        [JsonProperty("xp")]
        public long Xp { get; set; }
    }
}