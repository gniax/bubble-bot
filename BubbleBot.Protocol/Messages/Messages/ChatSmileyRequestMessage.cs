namespace BubbleBot.Protocol.Messages
{
    public class ChatSmileyRequestMessage : Message
    {

        // Properties
        public uint SmileyId { get; set; }


        // Constructors
        public ChatSmileyRequestMessage() { }

        public ChatSmileyRequestMessage(uint smileyId = 0)
        {
            SmileyId = smileyId;
        }

    }
}
