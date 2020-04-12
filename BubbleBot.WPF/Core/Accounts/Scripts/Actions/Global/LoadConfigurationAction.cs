using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.Configurations;
using System.IO;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    public class LoadConfigurationAction : ScriptAction
    {

        // Properties
        public string FileName { get; private set; }


        // Constructor
        public LoadConfigurationAction(string fileName)
        {
            FileName = fileName;
        }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            await Task.Delay(1);

            string path = Configuration.ConfigurationsPath + "\\" + FileName + ".config";
            if (File.Exists(path))
            {
                File.Copy(System.IO.Path.Combine(Configuration.ConfigurationsPath, $"{FileName}.config"),
                          System.IO.Path.Combine(Configuration.ConfigurationsPath, $"{account.AccountConfig.Username}.config"), overwrite: true);

                account.Configuration.Load();
                account.Logger.LogMessage(LanguageManager.Translate("165"), LanguageManager.Translate("629", (FileName + ".config")));

                return ScriptActionResults.DONE;
            }
            account.Logger.LogError(LanguageManager.Translate("165"), LanguageManager.Translate("628", FileName));
            return ScriptActionResults.FAILED;
        }

    }
}
