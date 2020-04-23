namespace BubbleBot.Protocol.Messages
{
    public class GameRolePlayTaxCollectorFightRequestMessage : Message
    {

        // Properties
        public int TaxCollectorId { get; set; }


        // Constructors
        public GameRolePlayTaxCollectorFightRequestMessage() { }

        public GameRolePlayTaxCollectorFightRequestMessage(int taxCollectorId = 0)
        {
            TaxCollectorId = taxCollectorId;
        }

    }
}
