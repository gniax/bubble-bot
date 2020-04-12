namespace BubbleBot.Protocol.Messages
{
    public class PrismInfoJoinLeaveRequestMessage : Message
    {

        // Properties
        public bool Join { get; set; }


        // Constructors
        public PrismInfoJoinLeaveRequestMessage() { }

        public PrismInfoJoinLeaveRequestMessage(bool join = false)
        {
            Join = join;
        }

    }
}
