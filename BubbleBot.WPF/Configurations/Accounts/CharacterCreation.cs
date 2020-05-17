using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace BubbleBot.Configurations
{
    public class CharacterCreation
    {
        // Properties
        [JsonProperty("Create")]
        public bool Create { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Server")]
        public string Server { get; set; }
        [JsonProperty("Breed")]
        public int Breed { get; set; }
        [JsonProperty("Sex")]
        public int Sex { get; set; }
        [JsonProperty("Head")]
        public int Head { get; set; }
        [JsonProperty("Colors")]
        public List<int> Colors { get; set; }
        [JsonProperty("ParametersToCopy")]
        public string ParametersToCopy { get; set; }
        [JsonProperty("FightsConfigurationToCopy")]
        public string FightsConfigurationToCopy { get; set; }
        [JsonProperty("CompleteTutorial")]
        public bool CompleteTutorial { get; set; }
    }
}