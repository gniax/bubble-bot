using Newtonsoft.Json;

namespace BubbleBot.Protocol.Data
{
    public class NpcMessages : IData
    {

        // Properties
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("messageId")]
        public string MessageId { get; set; }


        //Constructor
        internal NpcMessages() { }

    }
}
