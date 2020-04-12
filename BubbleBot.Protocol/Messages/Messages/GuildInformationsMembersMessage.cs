using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class GuildInformationsMembersMessage : Message
    {

        // Properties
        public List<GuildMember> Members { get; set; }


        // Constructors
        public GuildInformationsMembersMessage() { }

        public GuildInformationsMembersMessage(List<GuildMember> members = null)
        {
            Members = members;
        }

    }
}
