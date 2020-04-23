using Newtonsoft.Json;

namespace BubbleBot.Protocol.Data
{
    public class StealthBones : IData
    {

        // Properties
        [JsonProperty("id")]
        public int Id { get; set; }


        //Constructor
        internal StealthBones() { }

    }
}
