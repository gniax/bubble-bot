namespace BubbleBot.Protocol.Messages
{
    public class ExchangeClearPaymentForCraftMessage : Message
    {

        // Properties
        public int PaymentType { get; set; }


        // Constructors
        public ExchangeClearPaymentForCraftMessage() { }

        public ExchangeClearPaymentForCraftMessage(int paymentType = 0)
        {
            PaymentType = paymentType;
        }

    }
}
