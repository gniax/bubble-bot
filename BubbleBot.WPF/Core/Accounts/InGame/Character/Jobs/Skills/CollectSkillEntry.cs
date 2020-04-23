using BubbleBot.Protocol.Types;

namespace BubbleBot.Core.Accounts.InGame.Character.Jobs.Skills
{
    public class CollectSkillEntry
    {
        // Constructor
        public CollectSkillEntry(SkillActionDescriptionCollect skill, Protocol.Data.Skills skillData)
        {
            Id = skill.SkillId;
            InteractiveId = skillData.InteractiveId;
            Name = skillData.NameId;
            ParentJobId = skillData.ParentJobId;
        }

        // Properties
        public uint Id { get; }
        public int InteractiveId { get; }
        public string Name { get; }
        public int ParentJobId { get; }
    }
}