using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace BubbleBot.Views
{
    public partial class ExportFormatWindow
    {
        List<AccountConfiguration> Accounts;
        public ExportFormatWindow(List<AccountConfiguration> selectedAccounts)
        {
            Accounts = selectedAccounts;
            InitializeComponent();

            txtFormat.TextChanged += txtFormat_TextChanged;
        }

        private void txtFormat_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(spAlert.IsVisible)
            {
                spAlert.Visibility = Visibility.Hidden;
            }
        }

        private void btn_formatValidation(object sender, RoutedEventArgs e)
        {
            if(txtFormat.Text != null)
            {
                string[] separators = { ":" };
                string[] keys = txtFormat.Text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                List<string> listOutput = new List<string>();
                int error = 0;
                foreach (var key in keys)
                {
                    List<string> balises = new List<string>{ "username" , "password" , "ip" , "port" , "pxy-user" , "pxy-pass" , "id" };
                    if (balises.IndexOf(key.ToLower()) != -1)
                        listOutput.Add(key);
                    else
                        error++;
                }

                if (listOutput.Count == 0 || error > 0)
                {
                    SetAlert(LanguageManager.Translate("663", error), Visibility.Visible);
                    return;
                }

                List<string> accountsOutput = new List<string>();
                foreach(AccountConfiguration account in Accounts)
                {
                    string line = "";
                    foreach(var key in listOutput)
                    {
                        switch (key)
                        {
                            case "username":
                                line += account.Username + ':';
                                break;
                            case "password":
                                line += account.Password + ':';
                                break;
                            case "ip":
                                if(account.Proxy.Ip != "")
                                    line += account.Proxy.Ip + ':';
                                break;
                            case "port":
                                if (account.Proxy.Port != 0)
                                    line += account.Proxy.Port.ToString() + ':';
                                break;
                            case "pxy-user":
                                if (account.Proxy.Username != "")
                                    line += account.Proxy.Username + ':';
                                break;
                            case "pxy-pass":
                                if (account.Proxy.Password != "")
                                    line += account.Proxy.Password + ':';
                                break;
                            case "id":
                                if (account.Nickname != null)
                                    line += account.Nickname + ':';
                                break;
                            default:
                                error++;
                                break;
                        }
                    }
                    if(line.Length > 0)
                    {
                        // Remove the last ':' from the string
                        line = line.Remove(line.Length - 1);
                        accountsOutput.Add(line);
                    }
                    else
                    {
                        SetAlert(LanguageManager.Translate("664"), Visibility.Visible);
                        return;
                    }
                }

                if(accountsOutput.Count > 0)
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "Text file|*.txt";
                    saveFileDialog1.Title = LanguageManager.Translate("665");
                    saveFileDialog1.ShowDialog();

                    // If the file name is not an empty string open it for saving.
                    if (saveFileDialog1.FileName != "")
                    {
                        string[] output = accountsOutput.ToArray();
                        System.IO.File.WriteAllLines(saveFileDialog1.FileName, output);
                        tbAlert.Text = "";
                        spAlert.Visibility = Visibility.Hidden;
                        this.Close();
                    }
                }
                else
                {
                    SetAlert(LanguageManager.Translate("664"), Visibility.Visible);
                    return;
                }
            }

        }
        private void SetAlert(string text, Visibility visibility)
        {
            tbAlert.Text = text;
            spAlert.Visibility = visibility;
        }
    }
}

