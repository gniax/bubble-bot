namespace BubbleBot.Protocol.Messages
{
    public class AdminCommandMessage : Message
    {

        // Properties
        public string Content { get; set; }


        // Constructors
        public AdminCommandMessage() { }

        public AdminCommandMessage(string content = "")
        {
            Content = content;
        }

    }
}
