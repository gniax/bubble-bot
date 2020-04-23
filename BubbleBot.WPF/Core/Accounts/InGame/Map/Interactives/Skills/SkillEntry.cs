using BubbleBot.Protocol.Types;

namespace BubbleBot.Core.Accounts.InGame.Map.Interactives.Skills
{
    public class SkillEntry
    {
        // Constructor
        public SkillEntry(InteractiveElementSkill skill)
        {
            Id = (int) skill.SkillId;
            InstanceUID = skill.SkillInstanceUid;
            Name = skill.Name;
        }

        // Properties
        public int Id { get; }
        public uint InstanceUID { get; }
        public string Name { get; }
    }
}