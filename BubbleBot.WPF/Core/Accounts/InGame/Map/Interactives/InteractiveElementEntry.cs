using System.Collections.Generic;
using BubbleBot.Core.Accounts.InGame.Map.Interactives.Skills;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Core.Accounts.InGame.Map.Interactives
{
    public class InteractiveElementEntry
    {
        // Constructor
        public InteractiveElementEntry(InteractiveElement elem)
        {
            Id = elem.ElementId;
            ElementTypeId = elem.ElementTypeId;
            Name = elem.Name;

            EnabledSkills = new List<SkillEntry>(elem.EnabledSkills.Count);
            for (var i = 0; i < elem.EnabledSkills.Count; i++) EnabledSkills.Add(new SkillEntry(elem.EnabledSkills[i]));

            DisabledSkills = new List<SkillEntry>(elem.DisabledSkills.Count);
            for (var i = 0; i < elem.DisabledSkills.Count; i++)
                DisabledSkills.Add(new SkillEntry(elem.DisabledSkills[i]));
        }

        // Properties
        public uint Id { get; }
        public int ElementTypeId { get; }
        public string Name { get; }
        public List<SkillEntry> EnabledSkills { get; }
        public List<SkillEntry> DisabledSkills { get; }

        public bool Usable => EnabledSkills.Count > 0;
    }
}