using System;
using AntiRecaptcha;
using BubbleBot.Configurations;

namespace BubbleBot.Utility
{
    public class RecaptchaHandler
    {
        // Fields
        // private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);


        public string GetResponse(string siteKey)
        {
            // _semaphore.Wait();
            string result = null;

            if (!string.IsNullOrEmpty(GlobalConfiguration.Instance.AntiCaptchaKey))
                using (var ncp = new NoCaptchaProxyless(GlobalConfiguration.Instance.AntiCaptchaKey,
                    new Uri("https://proxyconnection.touch.dofus.com/recaptcha"), siteKey))
                {
                    ncp.CreateTask();
                    ncp.WaitForResult(300);

                    result = ncp.GetTaskSolution();
                }

            // _semaphore.Release();
            return result;
        }
    }
}