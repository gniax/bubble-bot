using Newtonsoft.Json;

namespace BubbleBot.Protocol.Data
{
    public class Tips : IData
    {

        // Properties
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("descId")]
        public string DescId { get; set; }


        //Constructor
        internal Tips() { }

    }
}
