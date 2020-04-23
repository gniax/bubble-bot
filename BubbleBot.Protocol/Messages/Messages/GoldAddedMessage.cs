using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class GoldAddedMessage : Message
    {

        // Properties
        public GoldItem Gold { get; set; }


        // Constructors
        public GoldAddedMessage() { }

        public GoldAddedMessage(GoldItem gold = null)
        {
            Gold = gold;
        }

    }
}
