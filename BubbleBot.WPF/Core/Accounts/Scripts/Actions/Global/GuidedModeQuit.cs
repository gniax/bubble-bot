using System;
using System.Threading;
using System.Threading.Tasks;
using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.Extensions.CharacterCreator;
using BubbleBot.Core.Enums;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    internal class GuidedModeQuit : ScriptAction
    {
        internal override async Task<ScriptActionResults> Process(Account account)
        {
            CharacterCreatorExtension.ActionStartTutorial(account);
            var tutorial = SpinWait.SpinUntil(() => account.Extensions.CharacterCreation._terminated,
                TimeSpan.FromSeconds(180));

            if (tutorial)
            {
                await Task.Delay(1200);
                account.Scripts.StopScript();
                account.Scripts.StartScript();
                return ScriptActionResults.PROCESSING;
            }

            account.Logger.LogInfo("Tutoriel", LanguageManager.Translate("656"));
            // Stocking previous data
            string prvServer = account.Game.Server.Name,
                prvParametersToCopy = account.AccountConfig.CharacterCreation.ParametersToCopy,
                prvFightsConfigurationToCopy = account.AccountConfig.CharacterCreation.FightsConfigurationToCopy;

            int prvBreed = (int) account.Game.Character.Breed,
                prvSex = account.Game.Character.Sex ? -1 : 0,
                prvHead = -1;

            var prvColors = account.Game.Character.Look.IndexedColors;

            // Here we have to re-create a character
            await account.Network.Disconnect("CLIENT_CLOSING");
            await Task.Delay(4000);
            account.Extensions.CharacterCreation.Clear();
            account.Game.Clear();

            var characterCreator = new CharacterCreation
            {
                Create = true,
                Name = "",
                Server = prvServer,
                Breed = prvBreed,
                Sex = prvSex,
                Head = prvHead,
                Colors = prvColors,
                ParametersToCopy = prvParametersToCopy,
                FightsConfigurationToCopy = prvFightsConfigurationToCopy,
                CompleteTutorial = false
            };
            account.AccountConfig.CharacterCreation = characterCreator;

            // Then we can reconnect the character and create a new one
            await account.Connect();
            await Task.Delay(2000);
            if (account.Network != null)
            {
                SpinWait.SpinUntil(() => account.Network.Phase == NetworkPhases.GAME, TimeSpan.FromSeconds(20));
                if (account.Network.Phase == NetworkPhases.GAME)
                {
                    await Task.Delay(5000);
                    account.Scripts.StartScript();
                    return ScriptActionResults.PROCESSING;
                }
            }

            return ScriptActionResults.FAILED;
        }
    }
}