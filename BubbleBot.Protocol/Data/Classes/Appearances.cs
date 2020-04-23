using Newtonsoft.Json;

namespace BubbleBot.Protocol.Data
{
    public class Appearances : IData
    {

        // Properties
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonProperty("data")]
        public string Data { get; set; }


        //Constructor
        internal Appearances() { }

    }
}
