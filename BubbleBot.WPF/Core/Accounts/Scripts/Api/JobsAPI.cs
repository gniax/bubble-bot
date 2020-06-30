using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoonSharp.Interpreter;

namespace BubbleBot.Core.Accounts.Scripts.Api
{
    [MoonSharpUserData]
    [Obfuscation(Exclude = false, Feature = "-rename", ApplyToMembers = true)]
    public class JobsAPI : IDisposable
    {
        // Fields
        public Account _account;


        // Constructor
        public JobsAPI(Account account)
        {
            _account = account;
        }


        public bool HasJob(int jobId)
        {
            return _account.Game.Character.Jobs.Jobs.FirstOrDefault(j => j.Id == jobId) != null;
        }

        public string Name(int jobId)
        {
            return _account.Game.Character.Jobs.Jobs.FirstOrDefault(j => j.Id == jobId)?.Name ?? "";
        }

        public uint Level(int jobId)
        {
            return _account.Game.Character.Jobs.Jobs.FirstOrDefault(j => j.Id == jobId)?.Level ?? 0;
        }

        public List<int> GetCollectSkills(int jobId)
        {
            var job = _account.Game.Character.Jobs.Jobs.FirstOrDefault(j => j.Id == jobId);

            if (job == null)
                return new List<int>();

            return job.CollectSkills.Select(f => f.InteractiveId).ToList();
        }

        public List<int> GetAllCollectSkills()
        {
            return _account.Game.Character.Jobs.GetCollectSkillsIds().ToList();
        }

        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _account = null;

                disposedValue = true;
            }
        }

        ~JobsAPI()
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