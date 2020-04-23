using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.InGame.Managers.Teleportables;
using BubbleBot.Utility.Extensions;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Map
{
    public class UseTeleportableAction : ScriptAction
    {
        // Constructor
        public UseTeleportableAction(Teleportables type, uint destinationMapId)
        {
            Type = type;
            DestinationMapId = destinationMapId;
        }

        // Properties
        public Teleportables Type { get; }
        public uint DestinationMapId { get; }

        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (Type == Teleportables.ZAAP && !account.Game.Managers.Teleportables.UseZaap(DestinationMapId) ||
                Type == Teleportables.ZAAPI && !account.Game.Managers.Teleportables.UseZaapi(DestinationMapId))
            {
                account.Scripts.StopScript(LanguageManager.Translate("540", Type.ToString().PureCapitalize()));
                return FailedResult;
            }

            return ProcessingResult;
        }
    }
}