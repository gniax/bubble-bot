namespace BubbleBot.Protocol.Types
{
    public class QuestActiveInformations
    {

        // Properties
        public uint QuestId { get; set; }


        // Constructors
        public QuestActiveInformations() { }

        public QuestActiveInformations(uint questId = 0)
        {
            QuestId = questId;
        }

    }
}
