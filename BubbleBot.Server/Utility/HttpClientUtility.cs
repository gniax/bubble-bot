using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BubbleBot.Server.Utility
{
    public static class HttpClientUtility
    {

        // Fields
        private static readonly HttpClient _httpClient;


        static HttpClientUtility()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(Constants.VpsApiIpAddress + "/api/")//"http://BubbleBot.fr/api/")
            };
        }


        public static async Task<JObject> GetJsonAsync(string url)
        {
            try
            {
                Console.WriteLine(url);
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseString = await response.Content.ReadAsStringAsync();
                return JObject.Parse(responseString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public static async Task PostAsync(string url, FormUrlEncodedContent content)
        {
            try
            {
                await _httpClient.PostAsync(url, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur PostAsync:" + ex);
            }
        }

        public static async Task PatchAsync(string url, FormUrlEncodedContent content)
        {
            try
            {
                await _httpClient.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), url)
                {
                    Content = content
                });
            }
            catch ( Exception ex )
            {
                Console.WriteLine("Erreur PatchAsync:" + ex);
            }
        }

        public static async Task DeleteAsync(string url)
        {
            try
            {
                await _httpClient.DeleteAsync(url);
            }
            catch { }
        }

    }
}
