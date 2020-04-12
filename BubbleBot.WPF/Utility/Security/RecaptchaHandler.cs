using AntiRecaptcha;
using BubbleBot.Configurations;
using System;

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
            {
                try
                {
                    using (NoCaptchaProxyless ncp = new NoCaptchaProxyless(GlobalConfiguration.Instance.AntiCaptchaKey, new Uri("https://proxyconnection.touch.dofus.com/recaptcha"), siteKey))
                    {
                        ncp.CreateTask();
                        ncp.WaitForResult(300);

                        result = ncp.GetTaskSolution();
                    }
                }
                catch
                {
                    //       _semaphore.Release();
                    throw;
                }
            }

            // _semaphore.Release();
            return result;
        }

    }
}
