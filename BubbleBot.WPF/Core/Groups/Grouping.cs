using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts;
using BubbleBot.Core.Accounts.InGame.Managers.Movements;

namespace BubbleBot.Core.Groups
{
    public class Grouping : IDisposable
    {
        // Fields
        private Group _group;
        private List<Account> _missingMembers;


        // Constructor
        public Grouping(Group group)
        {
            _group = group;
        }


        // Properties
        private int ChiefPosX => _group.Chief.Game.Map.PosX;
        private int ChiefPosY => _group.Chief.Game.Map.PosY;


        public async Task GroupMembers()
        {
            var missingMembers = _group.Members
                .Where(m => m.Game.Map.CurrentPosition != _group.Chief.Game.Map.CurrentPosition).ToList();
            if (missingMembers.Count == 0)
                return;

            _missingMembers = missingMembers;
            var groupings = new Task[_missingMembers.Count];
            for (var i = 0; i < _missingMembers.Count; i++) groupings[i] = GroupMissingMember(_missingMembers[i]);

            await Task.WhenAll(groupings);
        }

        private async Task GroupMissingMember(Account missingMember)
        {
            TaskCompletionSource<bool> tcs = null;

            async void MapChanged()
            {
                await Task.Delay(3000);
                tcs.SetResult(true);
            }

            missingMember.Game.Map.MapChanged += MapChanged;

            while (_group.Chief.Scripts.Enabled &&
                   missingMember.Game.Map.CurrentPosition != _group.Chief.Game.Map.CurrentPosition)
            {
                Console.WriteLine("test1");
                tcs = new TaskCompletionSource<bool>();
                MoveMissingMember(missingMember);
                await tcs.Task;
            }

            missingMember.Game.Map.MapChanged -= MapChanged;
        }

        private void MoveMissingMember(Account missingMember)
        {
            var dir = MapChangeDirections.NONE;

            // TOP
            if (ChiefPosY < missingMember.Game.Map.PosY)
                dir = MapChangeDirections.TOP;
            // BOTTOM
            else if (ChiefPosY > missingMember.Game.Map.PosY)
                dir = MapChangeDirections.BOTTOM;
            // LEFT
            else if (ChiefPosX < missingMember.Game.Map.PosX)
                dir = MapChangeDirections.LEFT;
            // RIGHT
            else if (ChiefPosX > missingMember.Game.Map.PosX) dir = MapChangeDirections.RIGHT;

            if (dir == MapChangeDirections.NONE)
                return;

            missingMember.Logger.LogDebug("Grouping", LanguageManager.Translate("173", dir.ToString().ToLower()));
            missingMember.Game.Managers.Movements.ChangeMap(dir);
        }

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _group = null;
                _missingMembers?.Clear();
                _missingMembers = null;

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}