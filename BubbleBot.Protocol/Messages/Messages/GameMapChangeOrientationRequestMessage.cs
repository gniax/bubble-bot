namespace BubbleBot.Protocol.Messages
{
    public class GameMapChangeOrientationRequestMessage : Message
    {

        // Properties
        public uint Direction { get; set; }


        // Constructors
        public GameMapChangeOrientationRequestMessage() { }

        public GameMapChangeOrientationRequestMessage(uint direction = 1)
        {
            Direction = direction;
        }

    }
}
