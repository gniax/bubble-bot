using Newtonsoft.Json;

namespace BubbleBot.Protocol.Data
{
    public class ServerPopulations : IData
    {

        // Properties
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("nameId")]
        public string NameId { get; set; }
        [JsonProperty("weight")]
        public int Weight { get; set; }


        //Constructor
        internal ServerPopulations() { }

    }
}
