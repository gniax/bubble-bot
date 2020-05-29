using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;

namespace BubbleBot.Views
{
    public partial class ImportFormatWindow
    { 
        private AccountsManagerWindow _parent;
        public ImportFormatWindow(AccountsManagerWindow parent)
        {
            InitializeComponent();
            _parent = parent;
            txtFormat.TextChanged += txtFormat_TextChanged;
            this.Closing += ImportFormatWindow_Closing;
        }

        private void ImportFormatWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_parent != null)
            {
                _parent.TxtCustomImportFormat.Text = "...";
                _parent.RdbtnCustom.IsChecked = false;
                _parent.RdbtnDefault.IsChecked = true;
            }
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
                        var balises = new List<string> { "username", "password", "ip", "port", "pxy-user", "pxy-pass", "id" };
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

                    string format = null;
                    foreach (var tag in listOutput)
                    {
                        format += $"{tag}{DelimiterPreview.Text}";
                    }

                    if (DelimiterPreview.Text != null && DelimiterPreview.Text != "" && format.EndsWith(DelimiterPreview.Text))
                        format = format.Remove(format.Length - 1);

                    if (_parent != null)
                    {
                        this.Closing -= ImportFormatWindow_Closing;
                        _parent.TxtCustomImportFormat.Text = format;
                    }

                    this.Close();
                                        
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

        private void SetAlert(string text, Visibility visibility)
        {
            tbAlert.Text = text;
            spAlert.Visibility = visibility;
        }
    }
}