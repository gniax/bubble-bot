namespace BubbleBot.Protocol.Types
{
    public class ObjectEffect
    {

        // Properties
        public uint Value { get; set; }
        public uint ActionId { get; set; }
        // Constructors
        public ObjectEffect() { }

        public ObjectEffect(uint actionId = 0, uint value = 0)
        {
            Value = value;
            ActionId = actionId;
        }

    }
}
