using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;

namespace BubbleBot.Views
{
    public partial class ExportFormatWindow
    {
        private readonly List<AccountConfiguration> Accounts;

        public ExportFormatWindow(List<AccountConfiguration> selectedAccounts)
        {
            Accounts = selectedAccounts;
            InitializeComponent();

            txtFormat.TextChanged += txtFormat_TextChanged;
        }

        private void txtFormat_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (spAlert.IsVisible) spAlert.Visibility = Visibility.Hidden;
            if (txtFormat.Text != null)
            {
                string pattern = @"^[a-zA-Z]+(\W)";
                var regMatch = Regex.Match(txtFormat.Text, pattern);
                if (regMatch.Success)
                {
                    DelimiterPreview.Text = regMatch.Groups[1].Value;
                }
                else
                {
                    DelimiterPreview.Text = null;
                }
            }
        }

        private void btn_formatValidation(object sender, RoutedEventArgs e)
        {
            if (txtFormat.Text != null)
            {
                if (DelimiterPreview.Text != null)
                {
                    string[] separators = new string[] { DelimiterPreview.Text };
                    var keys = txtFormat.Text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    var listOutput = new List<string>();
                    var error = 0;
                    foreach (var key in keys)
                    {
                        var balises = new List<string> {"username", "password", "ip", "port", "pxy-user", "pxy-pass", "id"};
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

                    var accountsOutput = new List<string>();
                    foreach (var account in Accounts)
                    {
                        var line = "";
                        foreach (var key in listOutput)
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
                                    if (account.Proxy.Ip != "")
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

                        if (line.Length > 0)
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

                    if (accountsOutput.Count > 0)
                    {
                        var saveFileDialog1 = new SaveFileDialog();
                        saveFileDialog1.Filter = "Text file|*.txt";
                        saveFileDialog1.Title = LanguageManager.Translate("665");
                        saveFileDialog1.ShowDialog();

                        // If the file name is not an empty string open it for saving.
                        if (saveFileDialog1.FileName != "")
                        {
                            var output = accountsOutput.ToArray();
                            File.WriteAllLines(saveFileDialog1.FileName, output);
                            tbAlert.Text = "";
                            spAlert.Visibility = Visibility.Hidden;
                            Close();
                        }
                    }
                    else
                    {
                        SetAlert(LanguageManager.Translate("664"), Visibility.Visible);
                    }
                }
                else
                {
                    SetAlert(LanguageManager.Translate("666"), Visibility.Visible);
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