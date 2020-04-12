namespace BubbleBot.Protocol.Messages
{
    public class GameFightTurnResumeMessage : GameFightTurnStartMessage
    {

        // Constructors
        public GameFightTurnResumeMessage() { }

        public GameFightTurnResumeMessage(int id = 0, uint waitTime = 0)
        {
            Id = id;
            WaitTime = waitTime;
        }

    }
}
