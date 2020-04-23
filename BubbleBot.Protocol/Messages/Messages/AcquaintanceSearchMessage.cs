namespace BubbleBot.Protocol.Messages
{
    public class AcquaintanceSearchMessage : Message
    {

        // Properties
        public string Nickname { get; set; }


        // Constructors
        public AcquaintanceSearchMessage() { }

        public AcquaintanceSearchMessage(string nickname = "")
        {
            Nickname = nickname;
        }

    }
}
