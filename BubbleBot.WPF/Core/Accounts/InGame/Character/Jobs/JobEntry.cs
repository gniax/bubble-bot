using System.Collections.Generic;
using System.Linq;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.InGame.Character.Jobs.Skills;
using BubbleBot.Protocol.Data;
using BubbleBot.Protocol.Types;
using BubbleBot.Utility.DofusTouch;
using GalaSoft.MvvmLight;
using BubbleBot.Data;

namespace BubbleBot.Core.Accounts.InGame.Character.Jobs
{
    public class JobEntry : ViewModelBase
    {
        // Fields
        private uint _level;


        // Constructor
        public JobEntry(JobDescription job, Protocol.Data.Jobs jobData)
        {
            Id = job.JobId;
            Name = jobData.NameId;
            IconId = jobData.IconId;
            CollectSkills = new List<CollectSkillEntry>();

            if (job.Skills.Count > 0)
            {
                var skills = DataManager.GetEnumerable<Protocol.Data.Skills>(job.Skills.Select(s => (int) s.SkillId));
                for (var i = 0; i < job.Skills.Count; i++)
                    if (job.Skills[i] is SkillActionDescriptionCollect)
                        CollectSkills.Add(new CollectSkillEntry(job.Skills[i] as SkillActionDescriptionCollect,
                            skills.FirstOrDefault(s => s.Id == job.Skills[i].SkillId)));
            }
        }


        // Properties
        public uint Id { get; }

        public uint Level
        {
            get => _level;
            set => Set(ref _level, value);
        }

        public string Name { get; }
        public int IconId { get; }
        public double Experience { get; private set; }
        public double ExperienceLevelFloor { get; private set; }
        public double ExperienceNextLevelFloor { get; private set; }
        public List<CollectSkillEntry> CollectSkills { get; }

        public int ExperiencePercent => Experience == 0
            ? 0
            : (int) ((Experience - ExperienceLevelFloor) / (ExperienceNextLevelFloor - ExperienceLevelFloor) * 100);

        public string IconUrl =>
            $"https://dofustouch.cdn.ankama.com/assets/{DTConstants.AssetsVersion}/gfx/jobs/{IconId}.png";


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