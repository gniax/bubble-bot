using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class JobCrafterDirectoryListMessage : Message
    {

        // Properties
        public List<JobCrafterDirectoryListEntry> ListEntries { get; set; }


        // Constructors
        public JobCrafterDirectoryListMessage() { }

        public JobCrafterDirectoryListMessage(List<JobCrafterDirectoryListEntry> listEntries = null)
        {
            ListEntries = listEntries;
        }

    }
}
