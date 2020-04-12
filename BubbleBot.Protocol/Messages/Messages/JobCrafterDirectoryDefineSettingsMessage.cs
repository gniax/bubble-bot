using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class JobCrafterDirectoryDefineSettingsMessage : Message
    {

        // Properties
        public JobCrafterDirectorySettings Settings { get; set; }


        // Constructors
        public JobCrafterDirectoryDefineSettingsMessage() { }

        public JobCrafterDirectoryDefineSettingsMessage(JobCrafterDirectorySettings settings = null)
        {
            Settings = settings;
        }

    }
}
