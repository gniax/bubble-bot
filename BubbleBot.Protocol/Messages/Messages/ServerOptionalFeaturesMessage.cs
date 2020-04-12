using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class ServerOptionalFeaturesMessage : Message
    {

        // Properties
        public List<uint> Features { get; set; }


        // Constructors
        public ServerOptionalFeaturesMessage() { }

        public ServerOptionalFeaturesMessage(List<uint> features = null)
        {
            Features = features;
        }

    }
}
