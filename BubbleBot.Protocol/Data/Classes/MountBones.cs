using Newtonsoft.Json;

namespace BubbleBot.Protocol.Data
{
    public class MountBones : IData
    {

        // Properties
        [JsonProperty("id")]
        public int Id { get; set; }


        //Constructor
        internal MountBones() { }

    }
}
