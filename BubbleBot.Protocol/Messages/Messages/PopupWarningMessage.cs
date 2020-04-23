namespace BubbleBot.Protocol.Messages
{
    public class PopupWarningMessage : Message
    {

        // Properties
        public uint LockDuration { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }


        // Constructors
        public PopupWarningMessage() { }

        public PopupWarningMessage(uint lockDuration = 0, string author = "", string content = "")
        {
            LockDuration = lockDuration;
            Author = author;
            Content = content;
        }

    }
}
