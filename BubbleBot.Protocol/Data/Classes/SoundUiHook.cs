using Newtonsoft.Json;

namespace BubbleBot.Protocol.Data
{
    public class SoundUiHook : IData
    {

        // Properties
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }


        //Constructor
        internal SoundUiHook() { }

    }
}
