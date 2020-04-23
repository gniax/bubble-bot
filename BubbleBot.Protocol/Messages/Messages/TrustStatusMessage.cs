namespace BubbleBot.Protocol.Messages
{
    public class TrustStatusMessage : Message
    {

        // Properties
        public bool Trusted { get; set; }


        // Constructors
        public TrustStatusMessage() { }

        public TrustStatusMessage(bool trusted = false)
        {
            Trusted = trusted;
        }

    }
}
