namespace BubbleBot.Protocol.Messages
{
    public class BasicPingMessage : Message
    {

        // Properties
        public bool Quiet { get; set; }


        // Constructors
        public BasicPingMessage() { }

        public BasicPingMessage(bool quiet = false)
        {
            Quiet = quiet;
        }

    }
}
