namespace BubbleBot.Protocol.Messages
{
    public class QuestStartRequestMessage : Message
    {

        // Properties
        public uint QuestId { get; set; }


        // Constructors
        public QuestStartRequestMessage() { }

        public QuestStartRequestMessage(uint questId = 0)
        {
            QuestId = questId;
        }

    }
}
