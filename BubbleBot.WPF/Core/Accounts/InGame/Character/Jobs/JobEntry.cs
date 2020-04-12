using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.InGame.Character.Jobs.Skills;
using BubbleBot.Protocol.Data;
using BubbleBot.Protocol.Types;
using BubbleBot.Utility.DofusTouch;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;

namespace BubbleBot.Core.Accounts.InGame.Character.Jobs
{
    public class JobEntry : ViewModelBase
    {

        // Fields
        private uint _level;


        // Properties
        public uint Id { get; private set; }
        public uint Level
        {
            get => _level;
            set => Set(ref _level, value);
        }
        public string Name { get; private set; }
        public int IconId { get; private set; }
        public double Experience { get; private set; }
        public double ExperienceLevelFloor { get; private set; }
        public double ExperienceNextLevelFloor { get; private set; }
        public List<CollectSkillEntry> CollectSkills { get; private set; }

        public int ExperiencePercent => Experience == 0 ? 0 : (int)((Experience - ExperienceLevelFloor) / (ExperienceNextLevelFloor - ExperienceLevelFloor) * 100);
        public string IconUrl => $"https://dofustouch.cdn.ankama.com/assets/{DTConstants.AssetsVersion}/gfx/jobs/{IconId}.png";


        // Constructor
        public JobEntry(JobDescription job, Protocol.Data.Jobs jobData)
        {
            Id = job.JobId;
            Name = jobData.NameId;
            IconId = jobData.IconId;
            CollectSkills = new List<CollectSkillEntry>();

            if (job.Skills.Count > 0)
            {
                var skills = DataManager.GetEnumerable<Protocol.Data.Skills>(job.Skills.Select(s => (int)s.SkillId));
                for (int i = 0; i < job.Skills.Count; i++)
                {
                    if (job.Skills[i] is SkillActionDescriptionCollect)
                    {
                        CollectSkills.Add(new CollectSkillEntry(job.Skills[i] as SkillActionDescriptionCollect, skills.FirstOrDefault(s => s.Id == job.Skills[i].SkillId)));
                    }
                }
            }
        }


        #region Updates

        public void Update(JobExperience message)
        {
            Level = message.JobLevel;
            Experience = message.JobXP;
            ExperienceLevelFloor = message.JobXpLevelFloor;
            ExperienceNextLevelFloor = message.JobXpNextLevelFloor;
            RaisePropertyChanged(LanguageManager.Translate("115"));
        }

        #endregion

    }
}
