using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BubbleBot.Protocol.Data;
using BubbleBot.Protocol.Messages;
using BubbleBot.Data;

namespace BubbleBot.Core.Accounts.InGame.Character.Jobs
{
    public class JobsGame : IDisposable
    {
        // Fields
        private Account _account;
        private bool _jobsInitialized;


        // Constructor
        public JobsGame(Account account)
        {
            _account = account;

            Jobs = new ObservableCollection<JobEntry>();
        }


        // Properties
        public ObservableCollection<JobEntry> Jobs { get; private set; }


        // Events
        public event Action JobsUpdated;


        public bool HasCollectSkill(int id)
        {
            return Jobs.FirstOrDefault(j => j.CollectSkills.FirstOrDefault(s => s.InteractiveId == id) != null) != null;
        }

        public IEnumerable<int> GetCollectSkillsIds()
        {
            return Jobs.SelectMany(job => job.CollectSkills.Select(s => s.InteractiveId));
        }

        #region Updates

        public void Update(JobDescriptionMessage message)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                _jobsInitialized = false;

                Jobs.Clear();
                var jobsData = await DataManager.GetEnumerableAsync<Protocol.Data.Jobs>(message.JobsDescription.Select(f => (int) f.JobId));
                for (var i = 0; i < message.JobsDescription.Count; i++)
                    Jobs.Add(new JobEntry(message.JobsDescription[i],
                        jobsData.FirstOrDefault(f => f.Id == message.JobsDescription[i].JobId)));

                _jobsInitialized = true;
            });

            JobsUpdated?.Invoke();
        }

        public async Task Update(JobExperienceMultiUpdateMessage message)
        {
            // Ugly fix
            while (!_jobsInitialized)
                await Task.Delay(50);

            for (var i = 0; i < message.ExperiencesUpdate.Count; i++)
                Jobs.FirstOrDefault(j => j.Id == message.ExperiencesUpdate[i].JobId)
                    ?.Update(message.ExperiencesUpdate[i]);

            JobsUpdated?.Invoke();
        }

        public void Update(JobExperienceUpdateMessage message)
        {
            Jobs.FirstOrDefault(j => j.Id == message.ExperiencesUpdate.JobId)?.Update(message.ExperiencesUpdate);

            JobsUpdated?.Invoke();
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Jobs = null;
                _account = null;

                disposedValue = true;
            }
        }

        ~JobsGame()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}