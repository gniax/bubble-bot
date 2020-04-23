using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class PlayerStatusUpdateRequestMessage : Message
    {

        // Properties
        public PlayerStatus Status { get; set; }


        // Constructors
        public PlayerStatusUpdateRequestMessage() { }

        public PlayerStatusUpdateRequestMessage(PlayerStatus status = null)
        {
            Status = status;
        }

    }
}
