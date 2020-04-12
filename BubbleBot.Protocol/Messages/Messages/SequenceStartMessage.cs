namespace BubbleBot.Protocol.Messages
{
    public class SequenceStartMessage : Message
    {

        // Properties
        public int SequenceType { get; set; }
        public int AuthorId { get; set; }


        // Constructors
        public SequenceStartMessage() { }

        public SequenceStartMessage(int sequenceType = 0, int authorId = 0)
        {
            SequenceType = sequenceType;
            AuthorId = authorId;
        }

    }
}
