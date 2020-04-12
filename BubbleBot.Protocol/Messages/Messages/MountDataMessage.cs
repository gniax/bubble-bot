using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class MountDataMessage : Message
    {

        // Properties
        public MountClientData MountData { get; set; }


        // Constructors
        public MountDataMessage() { }

        public MountDataMessage(MountClientData mountData = null)
        {
            MountData = mountData;
        }

    }
}
