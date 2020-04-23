using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class GameFightSynchronizeMessage : Message
    {

        // Properties
        public List<GameFightFighterInformations> Fighters { get; set; }


        // Constructors
        public GameFightSynchronizeMessage() { }

        public GameFightSynchronizeMessage(List<GameFightFighterInformations> fighters = null)
        {
            Fighters = fighters;
        }

    }
}
