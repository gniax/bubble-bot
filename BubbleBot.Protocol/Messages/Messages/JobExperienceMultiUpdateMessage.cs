using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class JobExperienceMultiUpdateMessage : Message
    {

        // Properties
        public List<JobExperience> ExperiencesUpdate { get; set; }


        // Constructors
        public JobExperienceMultiUpdateMessage() { }

        public JobExperienceMultiUpdateMessage(List<JobExperience> experiencesUpdate = null)
        {
            ExperiencesUpdate = experiencesUpdate;
        }

    }
}
