using AntiRecaptcha;
using System;
using System.Threading.Tasks;

namespace AccountGenerator.Core
{
    public class RecaptchaHandler
    {

        // Fields
        // private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<string> GetResponse(string siteKey)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            // _semaphore.Wait();
            string result = null;
            //6Leicx0TAAAAAE-R05fbh9qqtID2XDtkOBd7-KnF

            if (!string.IsNullOrEmpty(Program.API_KEY))
            {
                try
                {
                    using (NoCaptchaProxyless ncp = new NoCaptchaProxyless(Program.API_KEY, new Uri("https://haapi.ankama.com/json/Ankama/v2/Account/CreateGuest?game=18&lang=fr"), siteKey)) //https://www.dofus.com/fr/mmorpg/jouer
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