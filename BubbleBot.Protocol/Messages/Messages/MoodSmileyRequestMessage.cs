namespace BubbleBot.Protocol.Messages
{
    public class MoodSmileyRequestMessage : Message
    {

        // Properties
        public int SmileyId { get; set; }


        // Constructors
        public MoodSmileyRequestMessage() { }

        public MoodSmileyRequestMessage(int smileyId = 0)
        {
            SmileyId = smileyId;
        }

    }
}
