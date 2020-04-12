using Newtonsoft.Json;

namespace BubbleBot.Protocol.Data
{
    public class MonsterSuperRaces : IData
    {

        // Properties
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("nameId")]
        public string NameId { get; set; }


        //Constructor
        internal MonsterSuperRaces() { }

    }
}
