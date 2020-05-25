using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Navigation;
using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;
using MahApps.Metro.Controls.Dialogs;

namespace BubbleBot.Views
{
    public partial class OptionsWindow
    {
        // Constructor
        public OptionsWindow()
        {
            InitializeComponent();

            DataContext = GlobalConfiguration.Instance;
        }


        private void AntiCaptcha_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        #region ProxySettings

        private async void BtnTestProxy_Click(object sender, RoutedEventArgs e)
        {
            if (!ushort.TryParse(TxtProxyPort.Text, out var port))
                return;

            if (!IPAddress.TryParse(TxtProxyIp.Text, out var ip))
                ip = Dns.GetHostEntry(TxtProxyIp.Text).AddressList[0];

            if (ip == null)
                return;

            using (var http = new HttpClient(new HttpClientHandler
            {
                Proxy = new WebProxy($"http://{ip}:{port}", false)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(TxtProxyUsername.Text, TxtProxyPassword.Text)
                },
                PreAuthenticate = true,
                UseDefaultCredentials = false
            })
            { Timeout = new TimeSpan(0, 0, 5) })
            {
                try
                {
                    var response = await http.GetAsync("https://ipv4.icanhazip.com/");
                    response.EnsureSuccessStatusCode();

                    var text = await response.Content.ReadAsStringAsync();

                    // If the ip is valid, the proxy is working
                    if (text.Substring(0, text.Length - 1) == ip.ToString())
                    {
                        response = await http.GetAsync("https://proxyconnection.touch.dofus.com/haapi/getForumPostsList?lang=fr&topicId=24993");
                        if ((int)response.StatusCode == 403)
                        {
                            await this.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("731"));
                        }
                        else if ((int)response.StatusCode == 200)
                        {
                            await this.ShowMessageAsync(LanguageManager.Translate("357"), LanguageManager.Translate("355"));
                        }
                        else
                        {
                            await this.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("732", response.StatusCode));
                        }
                        return;
                    }
                }
                catch
                {
                }

                await this.ShowMessageAsync(LanguageManager.Translate("249"), LanguageManager.Translate("356"));
            }
        }

        private void BtnSaveProxy_Click(object sender, RoutedEventArgs e)
        {
            var ip = TxtProxyIp.Text.Length == 0 ? "" :
                IPAddress.TryParse(TxtProxyIp.Text, out var ipAddress) ? ipAddress.ToString() : null;

            if (ip == null || !ushort.TryParse(TxtProxyPort.Text, out var port))
                return;

            GlobalConfiguration.Instance.ProxyIp = TxtProxyIp.Text;
            GlobalConfiguration.Instance.ProxyPort = Convert.ToUInt16(TxtProxyPort.Text);
            GlobalConfiguration.Instance.ProxyUsername = TxtProxyUsername.Text;
            GlobalConfiguration.Instance.ProxyPassword = TxtProxyPassword.Text;
        }

        private void BtnResetProxy_Click(object sender, RoutedEventArgs e)
        {
            GlobalConfiguration.Instance.ProxyIp = "";
            GlobalConfiguration.Instance.ProxyPort = 0;
            GlobalConfiguration.Instance.ProxyUsername = "";
            GlobalConfiguration.Instance.ProxyPassword = "";
            
        }

        #endregion
    }
}