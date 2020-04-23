namespace BubbleBot.Protocol.Messages
{
    public class LockableUseCodeMessage : Message
    {

        // Properties
        public string Code { get; set; }


        // Constructors
        public LockableUseCodeMessage() { }

        public LockableUseCodeMessage(string code = "")
        {
            Code = code;
        }

    }
}
