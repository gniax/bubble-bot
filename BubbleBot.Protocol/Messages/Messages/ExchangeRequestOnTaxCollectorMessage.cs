namespace BubbleBot.Protocol.Messages
{
    public class ExchangeRequestOnTaxCollectorMessage : Message
    {

        // Properties
        public int TaxCollectorId { get; set; }


        // Constructors
        public ExchangeRequestOnTaxCollectorMessage() { }

        public ExchangeRequestOnTaxCollectorMessage(int taxCollectorId = 0)
        {
            TaxCollectorId = taxCollectorId;
        }

    }
}
