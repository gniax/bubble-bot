namespace BubbleBot.Protocol.Messages
{
    public class JobCrafterDirectoryEntryRequestMessage : Message
    {

        // Properties
        public uint PlayerId { get; set; }


        // Constructors
        public JobCrafterDirectoryEntryRequestMessage() { }

        public JobCrafterDirectoryEntryRequestMessage(uint playerId = 0)
        {
            PlayerId = playerId;
        }

    }
}
