using Newtonsoft.Json;

namespace BubbleBot.Core.Accounts.Configurations
{
    public class SpellToBoostEntry
    {
        // Constructor
        public SpellToBoostEntry(int spellId, string name, byte level)
        {
            Id = spellId;
            Name = name;
            Level = level;
        }

        // Properties
        [JsonProperty("SpellId")]
        public int Id { get; }
        [JsonProperty("Name")]
        public string Name { get; }
        [JsonProperty("Level")]
        public byte Level { get; }
    }
}