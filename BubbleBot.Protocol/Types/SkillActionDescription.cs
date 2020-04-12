namespace BubbleBot.Protocol.Types
{
    public class SkillActionDescription
    {

        // Properties
        public uint SkillId { get; set; }


        // Constructors
        public SkillActionDescription() { }

        public SkillActionDescription(uint skillId = 0)
        {
            SkillId = skillId;
        }

    }
}
