using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    class GuidedModeQuit : ScriptAction
    {
        internal async override Task<ScriptActionResults> Process(Account account)
        {
            BubbleBot.Core.Accounts.Extensions.CharacterCreator.CharacterCreatorExtension.ActionStartTutorial(account);
            bool tutorial = System.Threading.SpinWait.SpinUntil(() => (account.Extensions.CharacterCreation._terminated), TimeSpan.FromSeconds(180));

            if (tutorial)
            {
                await Task.Delay(1200);
                account.Scripts.StopScript();
                account.Scripts.StartScript();
                return ScriptActionResults.PROCESSING;
            }
            else // The bypass-tutorial failed
            {
                account.Logger.LogInfo("Tutoriel", LanguageManager.Translate("656"));
                // Stocking previous informations
                string prvServer = account.Game.Server.Name,
                       prvParametersToCopy = account.AccountConfig.CharacterCreation.ParametersToCopy,
                       prvFightsConfigurationToCopy = account.AccountConfig.CharacterCreation.FightsConfigurationToCopy;

                int prvBreed = (int)account.Game.Character.Breed,
                    prvSex = account.Game.Character.Sex ? -1 : 0,
                    prvHead = -1;

                List<int> prvColors = account.Game.Character.Look.IndexedColors;

                // Here we have to re-create a character
                await account.Network.Disconnect("CLIENT_CLOSING", false);
                await Task.Delay(4000);
                account.Extensions.CharacterCreation.Clear();
                account.Game.Clear();

                CharacterCreation characterCreator = new CharacterCreation()
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
                    SpinWait.SpinUntil(() => (account.Network.Phase == NetworkPhases.GAME), TimeSpan.FromSeconds(20));
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
}
