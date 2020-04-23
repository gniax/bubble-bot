using System.Threading.Tasks;
using BubbleBot.Core.Accounts;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Frames.Game
{
    public static class QuestsFrame
    {
        public static Task HandleQuestStartedMessage(Account account, QuestStartedMessage message)
        {
            return Task.Run(() => account.Extensions.CharacterCreation.Update(message));
        }

        public static Task HandleQuestStepInfoMessage(Account account, QuestStepInfoMessage message)
        {
            return Task.Run(() => account.Extensions.CharacterCreation.Update(message));
        }
        //=> Task.Run(async () => await account.Extensions.CharacterCreation.Update(message));

        public static Task HandleQuestStepValidatedMessage(Account account, QuestStepValidatedMessage message)
        {
            return Task.Run(() => account.Extensions.CharacterCreation.Update(message));
        }

        public static Task HandleQuestValidatedMessage(Account account, QuestValidatedMessage message)
        {
            return Task.Run(() => account.Extensions.CharacterCreation.Update(message));
        }
    }
}