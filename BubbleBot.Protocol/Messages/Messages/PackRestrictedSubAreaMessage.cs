namespace BubbleBot.Protocol.Messages
{
    public class PackRestrictedSubAreaMessage : Message
    {

        // Properties
        public uint SubAreaId { get; set; }


        // Constructors
        public PackRestrictedSubAreaMessage() { }

        public PackRestrictedSubAreaMessage(uint subAreaId = 0)
        {
            SubAreaId = subAreaId;
        }

    }
}
