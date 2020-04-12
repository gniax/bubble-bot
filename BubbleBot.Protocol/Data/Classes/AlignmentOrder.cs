using Newtonsoft.Json;

namespace BubbleBot.Protocol.Data
{
    public class AlignmentOrder : IData
    {

        // Properties
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("nameId")]
        public string NameId { get; set; }
        [JsonProperty("sideId")]
        public int SideId { get; set; }


        //Constructor
        internal AlignmentOrder() { }

    }
}
