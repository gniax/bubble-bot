using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Mount
{
    public class SetRatioAction : ScriptAction
    {
        // Constructor
        public SetRatioAction(uint ratio)
        {
            Ratio = ratio;
        }

        // Properties
        public uint Ratio { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Character.Mount.HasMount)
            {
                account.Game.Character.Mount.SetRatio(Ratio);
                await Task.Delay(400);
            }

            return ScriptActionResults.DONE;
        }
    }
}