using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class HousePropertiesMessage : Message
    {

        // Properties
        public HouseInformations Properties { get; set; }


        // Constructors
        public HousePropertiesMessage() { }

        public HousePropertiesMessage(HouseInformations properties = null)
        {
            Properties = properties;
        }

    }
}
