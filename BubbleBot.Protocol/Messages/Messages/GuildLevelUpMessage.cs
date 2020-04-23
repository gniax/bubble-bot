namespace BubbleBot.Protocol.Messages
{
    public class GuildLevelUpMessage : Message
    {

        // Properties
        public uint NewLevel { get; set; }


        // Constructors
        public GuildLevelUpMessage() { }

        public GuildLevelUpMessage(uint newLevel = 0)
        {
            NewLevel = newLevel;
        }

    }
}
