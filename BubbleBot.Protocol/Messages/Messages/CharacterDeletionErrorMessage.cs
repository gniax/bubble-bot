namespace BubbleBot.Protocol.Messages
{
    public class CharacterDeletionErrorMessage : Message
    {

        // Properties
        public uint Reason { get; set; }


        // Constructors
        public CharacterDeletionErrorMessage() { }

        public CharacterDeletionErrorMessage(uint reason = 1)
        {
            Reason = reason;
        }

    }
}
