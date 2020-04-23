namespace BubbleBot.Protocol.Messages
{
    public class BasicPongMessage : Message
    {

        // Properties
        public bool Quiet { get; set; }


        // Constructors
        public BasicPongMessage() { }

        public BasicPongMessage(bool quiet = false)
        {
            Quiet = quiet;
        }

    }
}
