using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Messages;
using System.Threading.Tasks;

namespace BubbleBot.Core.Frames.Game
{
    public static class QuestsFrame
    {

        public static Task HandleQuestStartedMessage(Account account, QuestStartedMessage message)
            => Task.Run(() => account.Extensions.CharacterCreation.Update(message));

        public static Task HandleQuestStepInfoMessage(Account account, QuestStepInfoMessage message)
            => Task.Run(() => account.Extensions.CharacterCreation.Update(message));
        //=> Task.Run(async () => await account.Extensions.CharacterCreation.Update(message));

        public static Task HandleQuestStepValidatedMessage(Account account, QuestStepValidatedMessage message)
            => Task.Run(() => account.Extensions.CharacterCreation.Update(message));

        public static Task HandleQuestValidatedMessage(Account account, QuestValidatedMessage message)
            => Task.Run(() => account.Extensions.CharacterCreation.Update(message));

    }
}
