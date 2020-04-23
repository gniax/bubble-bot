namespace BubbleBot.Protocol.Messages
{
    public class KamasUpdateMessage : Message
    {

        // Properties
        public int KamasTotal { get; set; }


        // Constructors
        public KamasUpdateMessage() { }

        public KamasUpdateMessage(int kamasTotal = 0)
        {
            KamasTotal = kamasTotal;
        }

    }
}
