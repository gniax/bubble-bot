namespace BubbleBot.Protocol.Messages
{
    public class StatsUpgradeResultMessage : Message
    {

        // Properties
        public uint NbCharacBoost { get; set; }


        // Constructors
        public StatsUpgradeResultMessage() { }

        public StatsUpgradeResultMessage(uint nbCharacBoost = 0)
        {
            NbCharacBoost = nbCharacBoost;
        }

    }
}
