namespace BubbleBot.Protocol.Messages
{
    public class CharacterReplayRequestMessage : Message
    {

        // Properties
        public uint CharacterId { get; set; }


        // Constructors
        public CharacterReplayRequestMessage() { }

        public CharacterReplayRequestMessage(uint characterId = 0)
        {
            CharacterId = characterId;
        }

    }
}
