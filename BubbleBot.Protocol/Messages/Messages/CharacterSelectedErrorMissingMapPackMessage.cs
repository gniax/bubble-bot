namespace BubbleBot.Protocol.Messages
{
    public class CharacterSelectedErrorMissingMapPackMessage : CharacterSelectedErrorMessage
    {

        // Properties
        public uint SubAreaId { get; set; }


        // Constructors
        public CharacterSelectedErrorMissingMapPackMessage() { }

        public CharacterSelectedErrorMissingMapPackMessage(uint subAreaId = 0)
        {
            SubAreaId = subAreaId;
        }

    }
}
