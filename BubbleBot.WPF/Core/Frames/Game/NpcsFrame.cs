using System.Threading.Tasks;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Frames.Game
{
    public static class NpcsFrame
    {
        public static Task HandleNpcDialogCreationMessage(Account account, NpcDialogCreationMessage message)
        {
            return Task.Run(() => account.Game.Npcs.Update(message));
        }

        public static Task HandleNpcDialogQuestionMessage(Account account, NpcDialogQuestionMessage message)
        {
            return Task.Run(() => account.Game.Npcs.Update(message));
        }

        public static Task HandleLeaveDialogMessage(Account account, LeaveDialogMessage message)
        {
            return Task.Run(() => account.Game.Npcs.Update(message));
        }
    }
}