using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class GameFightShowFighterRandomStaticPoseMessage : GameFightShowFighterMessage
    {

        // Constructors
        public GameFightShowFighterRandomStaticPoseMessage() { }

        public GameFightShowFighterRandomStaticPoseMessage(GameFightFighterInformations informations = null)
        {
            Informations = informations;
        }

    }
}
