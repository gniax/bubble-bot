using BubbleBot.Protocol.Converters;
using BubbleBot.Protocol.Types;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class GameFightSynchronizeMessage : Message
    {

        // Properties
        [JsonConverter(typeof(TypedPropertyConverter))]
        public List<GameFightFighterInformations> Fighters { get; set; }


        // Constructors
        public GameFightSynchronizeMessage() { }

        public GameFightSynchronizeMessage(List<GameFightFighterInformations> fighters = null)
        {
            Fighters = fighters;
        }

    }
}
