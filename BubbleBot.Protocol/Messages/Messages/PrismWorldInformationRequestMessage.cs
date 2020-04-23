namespace BubbleBot.Protocol.Messages
{
    public class PrismWorldInformationRequestMessage : Message
    {

        // Properties
        public bool Join { get; set; }


        // Constructors
        public PrismWorldInformationRequestMessage() { }

        public PrismWorldInformationRequestMessage(bool join = false)
        {
            Join = join;
        }

    }
}
