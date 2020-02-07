using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.Extensions.Fights.Configuration;
using BubbleBot.Views.Accounts;
using System.IO;
using System.Threading.Tasks;
using BubbleBot.WPF.Views;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    public class LoadFightConfigurationAction : ScriptAction
    {

        // Properties
        public string FileName { get; private set; }


        // Constructor
        public LoadFightConfigurationAction(string fileName)
        {
            FileName = fileName;
        }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            string path = FightsConfiguration.ConfigurationsPath + "\\" + FileName + ".fconfig";
            if (File.Exists(path))
            {
                File.Copy(System.IO.Path.Combine(FightsConfiguration.ConfigurationsPath, $"{FileName}.fconfig"),
                          System.IO.Path.Combine(FightsConfiguration.ConfigurationsPath, $"{account.AccountConfig.Username}.fconfig"), overwrite: true);

                account.Extensions.Fights.Configuration.Load();
                account.Logger.LogMessage(LanguageManager.Translate("165"), LanguageManager.Translate("629", (FileName + ".fconfig")));
                return ScriptActionResults.DONE;
            }
            account.Logger.LogError(LanguageManager.Translate("165"), LanguageManager.Translate("627", FileName));
            return ScriptActionResults.FAILED;
        }

    }
}
