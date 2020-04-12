namespace BubbleBot.Protocol.Messages
{
    public class SubscriptionZoneMessage : Message
    {

        // Properties
        public bool Active { get; set; }


        // Constructors
        public SubscriptionZoneMessage() { }

        public SubscriptionZoneMessage(bool active = false)
        {
            Active = active;
        }

    }
}
