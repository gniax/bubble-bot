namespace BubbleBot.Protocol.Messages
{
    public class HouseKickIndoorMerchantRequestMessage : Message
    {

        // Properties
        public uint CellId { get; set; }


        // Constructors
        public HouseKickIndoorMerchantRequestMessage() { }

        public HouseKickIndoorMerchantRequestMessage(uint cellId = 0)
        {
            CellId = cellId;
        }

    }
}
