using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class JobCrafterDirectorySettingsMessage : Message
    {

        // Properties
        public List<JobCrafterDirectorySettings> CraftersSettings { get; set; }


        // Constructors
        public JobCrafterDirectorySettingsMessage() { }

        public JobCrafterDirectorySettingsMessage(List<JobCrafterDirectorySettings> craftersSettings = null)
        {
            CraftersSettings = craftersSettings;
        }

    }
}
