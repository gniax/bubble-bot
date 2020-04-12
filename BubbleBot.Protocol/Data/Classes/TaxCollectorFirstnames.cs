using Newtonsoft.Json;

namespace BubbleBot.Protocol.Data
{
    public class TaxCollectorFirstnames : IData
    {

        // Properties
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("firstnameId")]
        public string FirstnameId { get; set; }


        //Constructor
        internal TaxCollectorFirstnames() { }

    }
}
