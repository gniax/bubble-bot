namespace BubbleBot.Protocol.Messages
{
    public class ClientKeyMessage : Message
    {

        // Properties
        public string Key { get; set; }


        // Constructors
        public ClientKeyMessage() { }

        public ClientKeyMessage(string key = "")
        {
            Key = key;
        }

    }
}
