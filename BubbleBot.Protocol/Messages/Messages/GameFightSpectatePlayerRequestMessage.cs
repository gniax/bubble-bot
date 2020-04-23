namespace BubbleBot.Protocol.Messages
{
    public class GameFightSpectatePlayerRequestMessage : Message
    {

        // Properties
        public int PlayerId { get; set; }


        // Constructors
        public GameFightSpectatePlayerRequestMessage() { }

        public GameFightSpectatePlayerRequestMessage(int playerId = 0)
        {
            PlayerId = playerId;
        }

    }
}
