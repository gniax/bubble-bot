namespace BubbleBot.Protocol.Messages
{
    public class IdentificationFailedBannedMessage : IdentificationFailedMessage
    {

        // Properties
        public double BanEndDate { get; set; }


        // Constructors
        public IdentificationFailedBannedMessage() { }

        public IdentificationFailedBannedMessage(uint reason = 99, double banEndDate = 0)
        {
            Reason = reason;
            BanEndDate = banEndDate;
        }

    }
}
