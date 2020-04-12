namespace BubbleBot.Protocol.Messages
{
    public class ObjectUseOnCellMessage : ObjectUseMessage
    {

        // Properties
        public uint Cells { get; set; }


        // Constructors
        public ObjectUseOnCellMessage() { }

        public ObjectUseOnCellMessage(uint objectUID = 0, uint cells = 0)
        {
            ObjectUID = objectUID;
            Cells = cells;
        }

    }
}
