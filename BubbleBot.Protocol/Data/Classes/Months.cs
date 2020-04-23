using Newtonsoft.Json;

namespace BubbleBot.Protocol.Data
{
    public class Months : IData
    {

        // Properties
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("nameId")]
        public string NameId { get; set; }


        //Constructor
        internal Months() { }

    }
}
