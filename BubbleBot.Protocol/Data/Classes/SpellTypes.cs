using Newtonsoft.Json;

namespace BubbleBot.Protocol.Data
{
    public class SpellTypes : IData
    {

        // Properties
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("longNameId")]
        public string LongNameId { get; set; }
        [JsonProperty("shortNameId")]
        public string ShortNameId { get; set; }


        //Constructor
        internal SpellTypes() { }

    }
}
