using Newtonsoft.Json;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Data
{
    public class SpeakingItemsTriggers : IData
    {

        // Properties
        [JsonProperty("triggersId")]
        public int Id { get; set; }
        [JsonProperty("textIds")]
        public List<int> TextIds { get; set; }
        [JsonProperty("states")]
        public List<int> States { get; set; }


        //Constructor
        internal SpeakingItemsTriggers() { }

    }
}
