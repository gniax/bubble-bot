namespace BubbleBot.Protocol.Messages
{
    public class BasicWhoIsNoMatchMessage : Message
    {

        // Properties
        public string Search { get; set; }


        // Constructors
        public BasicWhoIsNoMatchMessage() { }

        public BasicWhoIsNoMatchMessage(string search = "")
        {
            Search = search;
        }

    }
}
