using Newtonsoft.Json;

namespace BubbleBot.Protocol.Data
{
    public class Mounts : IData
    {

        // Properties
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("nameId")]
        public string NameId { get; set; }
        [JsonProperty("look")]
        public string Look { get; set; }


        //Constructor
        internal Mounts() { }

    }
}
