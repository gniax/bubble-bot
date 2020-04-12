using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class GuildInformationsMemberUpdateMessage : Message
    {

        // Properties
        public GuildMember Member { get; set; }


        // Constructors
        public GuildInformationsMemberUpdateMessage() { }

        public GuildInformationsMemberUpdateMessage(GuildMember member = null)
        {
            Member = member;
        }

    }
}
