namespace BubbleBot.Protocol.Messages
{
    public class KrosmasterAuthTokenMessage : Message
    {

        // Properties
        public string Token { get; set; }


        // Constructors
        public KrosmasterAuthTokenMessage() { }

        public KrosmasterAuthTokenMessage(string token = "")
        {
            Token = token;
        }

    }
}
