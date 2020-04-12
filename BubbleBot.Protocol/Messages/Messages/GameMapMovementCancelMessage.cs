namespace BubbleBot.Protocol.Messages
{
    public class GameMapMovementCancelMessage : Message
    {

        // Properties
        public uint CellId { get; set; }


        // Constructors
        public GameMapMovementCancelMessage() { }

        public GameMapMovementCancelMessage(uint cellId = 0)
        {
            CellId = cellId;
        }

    }
}
