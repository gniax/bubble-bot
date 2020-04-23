namespace BubbleBot.Protocol.Messages
{
    public class CinematicMessage : Message
    {

        // Properties
        public uint CinematicId { get; set; }


        // Constructors
        public CinematicMessage() { }

        public CinematicMessage(uint cinematicId = 0)
        {
            CinematicId = cinematicId;
        }

    }
}
