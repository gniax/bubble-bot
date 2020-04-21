using System.IO;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.Extensions.Fights.Configuration;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    public class LoadFightConfigurationAction : ScriptAction
    {
        // Constructor
        public LoadFightConfigurationAction(string fileName)
        {
            FileName = fileName;
        }

        // Properties
        public string FileName { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            await Task.Delay(1);
            var path = FightsConfiguration.ConfigurationsPath + "\\" + FileName + ".fconfig";
            if (File.Exists(path))
            {
                File.Copy(Path.Combine(FightsConfiguration.ConfigurationsPath, $"{FileName}.fconfig"),
                    Path.Combine(FightsConfiguration.ConfigurationsPath, $"{account.AccountConfig.Username}.fconfig"),
                    true);

                account.Extensions.Fights.Configuration.Load();
                account.Logger.LogMessage(LanguageManager.Translate("165"),
                    LanguageManager.Translate("629", FileName + ".fconfig"));
                return ScriptActionResults.DONE;
            }

            account.Logger.LogError(LanguageManager.Translate("165"), LanguageManager.Translate("627", FileName));
            return ScriptActionResults.FAILED;
        }
    }
}