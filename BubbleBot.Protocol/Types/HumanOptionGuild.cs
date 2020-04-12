namespace BubbleBot.Protocol.Types
{
    public class HumanOptionGuild : HumanOption
    {

        // Properties
        public GuildInformations GuildInformations { get; set; }


        // Constructors
        public HumanOptionGuild() { }

        public HumanOptionGuild(GuildInformations guildInformations = null)
        {
            GuildInformations = guildInformations;
        }

    }
}
