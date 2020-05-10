using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;

namespace BubbleBot.Website.Extensions
{
    public static class CaptchaExtension
    {
        public static bool ReCaptchaPassed(string gRecaptchaResponse)
        {
          /*  HttpClient httpClient = new HttpClient();
            var res = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret=RECAPTCHA_SECRET_KEY&response={gRecaptchaResponse}").Result;
            if (res.StatusCode != HttpStatusCode.OK)
                return false;

            string JSONres = res.Content.ReadAsStringAsync().Result;
            dynamic JSONdata = JObject.Parse(JSONres);
            if (JSONdata.success != "true")
                return false;
                */
            return true;
        }
    }
}
