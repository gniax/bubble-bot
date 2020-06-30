using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BubbleBot.Views
{
    public partial class AddSubstituteAccounts
    {
        public AccountsManagerWindow WParent { get; set; }
        public AddSubstituteAccounts(AccountsManagerWindow parent)
        {
            InitializeComponent();

            WParent = parent;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (TxtContent.Text == null || TxtContent.Text == "" || TxtContent.Text == " ")
                return;

            string[] delimiter = { Environment.NewLine };
            string[] lines = TxtContent.Text.Split(delimiter, StringSplitOptions.None);
            short count = 0;

            foreach (var line in lines)
            {
                string cleanLine = line.Trim();
                string[] splittedLine = line.Split(':');

                if (splittedLine[0] != null && splittedLine[0] != "" && !splittedLine[0].Contains(":") && splittedLine[0] != line)
                {
                    if (splittedLine[1] != null && splittedLine[1] != "" && !splittedLine[1].Contains(":"))
                    {
                        GlobalConfiguration.Instance.AddAccountAndSave(splittedLine[0], splittedLine[1]);
                        count++;
                    }
                }
            }

            if (count > 0)
            {
                WParent?.UpdateAvailableSubstituteAccounts();
                WParent?.ShowMessageAsync(LanguageManager.Translate("668"), LanguageManager.Translate("497", count));
                Close();
            }
            else
            {
                this.ShowMessageAsync(LanguageManager.Translate("668"), LanguageManager.Translate("768"));
            }


        }
    }

}
