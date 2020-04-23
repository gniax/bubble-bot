using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.InGame.Managers.Movements;
using BubbleBot.Utility;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Map
{
    public class ChangeMapAction : ScriptAction
    {
        // Constructor
        internal ChangeMapAction(MapChangeDirections direction, short cellId)
        {
            Direction = direction;
            CellId = cellId;
        }

        // Properties
        public MapChangeDirections Direction { get; }
        public short CellId { get; }

        public bool IsSpecificDirection => Direction != MapChangeDirections.NONE && CellId != -1;
        public bool IsSimpleDirection => Direction != MapChangeDirections.NONE && CellId == -1;

        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (IsSpecificDirection)
            {
                if (!account.Game.Managers.Movements.ChangeMap(Direction, CellId))
                {
                    account.Scripts.StopScript(LanguageManager.Translate("170", Direction.ToString().ToLower(),
                        CellId));
                    return FailedResult;
                }

                account.Logger.LogDebug(LanguageManager.Translate("165"),
                    LanguageManager.Translate("171", Direction.ToString().ToLower(), CellId));
            }
            else if (IsSimpleDirection)
            {
                if (!account.Game.Managers.Movements.ChangeMap(Direction))
                {
                    account.Scripts.StopScript(LanguageManager.Translate("172", Direction.ToString().ToLower()));
                    return FailedResult;
                }

                account.Logger.LogDebug(LanguageManager.Translate("165"),
                    LanguageManager.Translate("173", Direction.ToString().ToLower()));
            }
            else // Move to a cell that will change the map
            {
                var result = account.Game.Managers.Movements.MoveToCell(CellId);

                if (result != MovementRequestResults.MOVED)
                {
                    account.Scripts.StopScript(LanguageManager.Translate("174", CellId, result));
                    return FailedResult;
                }

                account.Logger.LogDebug(LanguageManager.Translate("165"), LanguageManager.Translate("175", CellId));
            }

            return ProcessingResult;
        }

        public static bool TryParse(string text, out ChangeMapAction action)
        {
            var parts = text.Split('|');
            var randomPart = parts[Randomize.GetRandomInt(0, parts.Length)];

            // Specific direction
            var m = Regex.Match(randomPart,
                @"(?<direction>top|haut|right|droite|bottom|bas|left|gauche)\((?<cellId>\d{1,3})\)");
            if (m.Success)
            {
                action = new ChangeMapAction(
                    (MapChangeDirections) Enum.Parse(typeof(MapChangeDirections), m.Groups["direction"].Value, true),
                    Convert.ToInt16(m.Groups["cellId"].Value));
                return true;
            }

            // Simple directions
            m = Regex.Match(randomPart, @"(?<direction>top|haut|right|droite|bottom|bas|left|gauche)");
            if (m.Success)
            {
                action = new ChangeMapAction(
                    (MapChangeDirections) Enum.Parse(typeof(MapChangeDirections), m.Groups["direction"].Value, true),
                    -1);
                return true;
            }

            // Change maps from cells
            m = Regex.Match(randomPart, @"(?<cellId>\d{1,3})");
            if (m.Success)
            {
                action = new ChangeMapAction(MapChangeDirections.NONE, Convert.ToInt16(m.Groups["cellId"].Value));
                return true;
            }

            action = null;
            return false;
        }
    }
}