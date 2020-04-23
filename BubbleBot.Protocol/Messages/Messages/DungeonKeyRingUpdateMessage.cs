namespace BubbleBot.Protocol.Messages
{
    public class DungeonKeyRingUpdateMessage : Message
    {

        // Properties
        public uint DungeonId { get; set; }
        public bool Available { get; set; }


        // Constructors
        public DungeonKeyRingUpdateMessage() { }

        public DungeonKeyRingUpdateMessage(uint dungeonId = 0, bool available = false)
        {
            DungeonId = dungeonId;
            Available = available;
        }

    }
}
