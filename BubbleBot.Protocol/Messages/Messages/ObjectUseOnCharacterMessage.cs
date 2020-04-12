namespace BubbleBot.Protocol.Messages
{
    public class ObjectUseOnCharacterMessage : ObjectUseMessage
    {

        // Properties
        public uint CharacterId { get; set; }


        // Constructors
        public ObjectUseOnCharacterMessage() { }

        public ObjectUseOnCharacterMessage(uint objectUID = 0, uint characterId = 0)
        {
            ObjectUID = objectUID;
            CharacterId = characterId;
        }

    }
}
