using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class IgnoredListMessage : Message
    {

        // Properties
        public List<IgnoredInformations> IgnoredList { get; set; }


        // Constructors
        public IgnoredListMessage() { }

        public IgnoredListMessage(List<IgnoredInformations> ignoredList = null)
        {
            IgnoredList = ignoredList;
        }

    }
}
