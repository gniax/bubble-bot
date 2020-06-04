using System;
using System.Threading;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    internal class GroupMapTeleportationAction : ScriptAction
    {
        // Constructor
        public GroupMapTeleportationAction(int destMapId)
        {
            DestMapId = destMapId;
        }

        // Properties
        public int DestMapId { get; }

        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (account.HasGroup && !account.IsGroupChief)
                return DoneResult;

            foreach (var acc in account.Group.Members)
            {
                SpinWait.SpinUntil(() => acc.Game.Map.Id == DestMapId, TimeSpan.FromSeconds(20));
            }
            SpinWait.SpinUntil(() => account.Game.Map.Id == DestMapId, TimeSpan.FromSeconds(20));


            return DoneResult;
        }
    }
}