using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class BasicCharactersListMessage : Message
    {

        // Properties
        public List<CharacterBaseInformations> Characters { get; set; }


        // Constructors
        public BasicCharactersListMessage() { }

        public BasicCharactersListMessage(List<CharacterBaseInformations> characters = null)
        {
            Characters = characters;
        }

    }
}
