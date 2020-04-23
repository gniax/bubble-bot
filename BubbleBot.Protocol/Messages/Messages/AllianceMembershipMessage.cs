using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class AllianceMembershipMessage : AllianceJoinedMessage
    {

        // Constructors
        public AllianceMembershipMessage() { }

        public AllianceMembershipMessage(AllianceInformations allianceInfo = null, bool enabled = false)
        {
            AllianceInfo = allianceInfo;
            Enabled = enabled;
        }

    }
}
