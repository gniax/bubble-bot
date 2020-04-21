using System.IO;
using System.Threading.Tasks;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.Configurations;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    public class LoadConfigurationAction : ScriptAction
    {
        // Constructor
        public LoadConfigurationAction(string fileName)
        {
            FileName = fileName;
        }

        // Properties
        public string FileName { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            await Task.Delay(1);

            var path = Configuration.ConfigurationsPath + "\\" + FileName + ".config";
            if (File.Exists(path))
            {
                File.Copy(Path.Combine(Configuration.ConfigurationsPath, $"{FileName}.config"),
                    Path.Combine(Configuration.ConfigurationsPath, $"{account.AccountConfig.Username}.config"), true);

                account.Configuration.Load();
                account.Logger.LogMessage(LanguageManager.Translate("165"),
                    LanguageManager.Translate("629", FileName + ".config"));

                return ScriptActionResults.DONE;
            }

            account.Logger.LogError(LanguageManager.Translate("165"), LanguageManager.Translate("628", FileName));
            return ScriptActionResults.FAILED;
        }
    }
}