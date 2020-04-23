using BubbleBot.Protocol.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Types
{
    public class BidExchangerObjectInfo
    {

        // Properties
        [JsonConverter(typeof(TypedPropertyConverter))]
        public List<ObjectEffect> Effects { get; set; }
        public List<uint> Prices { get; set; }
        public uint ObjectUID { get; set; }


        // Constructors
        public BidExchangerObjectInfo() { }

        public BidExchangerObjectInfo(uint objectUID = 0, List<ObjectEffect> effects = null, List<uint> prices = null)
        {
            ObjectUID = objectUID;
            Effects = effects;
            Prices = prices;
        }

    }
}
