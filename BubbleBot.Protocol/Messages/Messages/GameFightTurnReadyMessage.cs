namespace BubbleBot.Protocol.Messages
{
    public class GameFightTurnReadyMessage : Message
    {

        // Properties
        public bool IsReady { get; set; }


        // Constructors
        public GameFightTurnReadyMessage() { }

        public GameFightTurnReadyMessage(bool isReady = false)
        {
            IsReady = isReady;
        }

    }
}
