namespace BubbleBot.Protocol.Messages
{
    public class ConsoleMessage : Message
    {

        // Properties
        public uint Type { get; set; }
        public string Content { get; set; }


        // Constructors
        public ConsoleMessage() { }

        public ConsoleMessage(uint type = 0, string content = "")
        {
            Type = type;
            Content = content;
        }

    }
}
