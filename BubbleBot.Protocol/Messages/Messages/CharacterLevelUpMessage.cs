namespace BubbleBot.Protocol.Messages
{
    public class CharacterLevelUpMessage : Message
    {

        // Properties
        public uint NewLevel { get; set; }


        // Constructors
        public CharacterLevelUpMessage() { }

        public CharacterLevelUpMessage(uint newLevel = 0)
        {
            NewLevel = newLevel;
        }

    }
}
