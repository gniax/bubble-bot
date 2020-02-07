using System;
using System.Collections.Generic;
using System.Text;
using AntiRecaptcha;
using System.Threading;
using System.Threading.Tasks;

namespace AccountGenerator.Core
{
    public class RecaptchaHandler
    {

        // Fields
        // private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        public async Task<string> GetResponse(string siteKey)
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