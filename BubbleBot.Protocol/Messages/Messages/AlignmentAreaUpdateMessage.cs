namespace BubbleBot.Protocol.Messages
{
    public class AlignmentAreaUpdateMessage : Message
    {

        // Properties
        public uint AreaId { get; set; }
        public int Side { get; set; }


        // Constructors
        public AlignmentAreaUpdateMessage() { }

        public AlignmentAreaUpdateMessage(uint areaId = 0, int side = 0)
        {
            AreaId = areaId;
            Side = side;
        }

    }
}
