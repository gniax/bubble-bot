namespace BubbleBot.Protocol.Messages
{
    public class LifePointsRegenEndMessage : UpdateLifePointsMessage
    {

        // Properties
        public uint LifePointsGained { get; set; }


        // Constructors
        public LifePointsRegenEndMessage() { }

        public LifePointsRegenEndMessage(uint lifePoints = 0, uint maxLifePoints = 0, uint lifePointsGained = 0)
        {
            LifePoints = lifePoints;
            MaxLifePoints = maxLifePoints;
            LifePointsGained = lifePointsGained;
        }

    }
}
