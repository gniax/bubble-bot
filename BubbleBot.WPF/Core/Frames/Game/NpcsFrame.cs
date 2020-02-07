using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Messages;
using System.Threading.Tasks;

namespace BubbleBot.Core.Frames.Game
{
    public static class NpcsFrame
    {

        public static Task HandleNpcDialogCreationMessage(Account account, NpcDialogCreationMessage message)
            => Task.Run(() => account.Game.Npcs.Update(message));

        public static Task HandleNpcDialogQuestionMessage(Account account, NpcDialogQuestionMessage message)
            => Task.Run(() => account.Game.Npcs.Update(message));

        public static Task HandleLeaveDialogMessage(Account account, LeaveDialogMessage message)
            => Task.Run(() => account.Game.Npcs.Update(message));

    }
}
