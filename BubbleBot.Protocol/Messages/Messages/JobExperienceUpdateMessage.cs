using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class JobExperienceUpdateMessage : Message
    {

        // Properties
        public JobExperience ExperiencesUpdate { get; set; }


        // Constructors
        public JobExperienceUpdateMessage() { }

        public JobExperienceUpdateMessage(JobExperience experiencesUpdate = null)
        {
            ExperiencesUpdate = experiencesUpdate;
        }

    }
}
