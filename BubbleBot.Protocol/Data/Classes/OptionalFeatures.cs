using Newtonsoft.Json;

namespace BubbleBot.Protocol.Data
{
    public class OptionalFeatures : IData
    {

        // Properties
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("keyword")]
        public string Keyword { get; set; }


        //Constructor
        internal OptionalFeatures() { }

    }
}
