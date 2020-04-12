using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class CharacterReplayWithRecolorRequestMessage : CharacterReplayRequestMessage
    {

        // Properties
        public List<int> IndexedColor { get; set; }


        // Constructors
        public CharacterReplayWithRecolorRequestMessage() { }

        public CharacterReplayWithRecolorRequestMessage(uint characterId = 0, List<int> indexedColor = null)
        {
            CharacterId = characterId;
            IndexedColor = indexedColor;
        }

    }
}
