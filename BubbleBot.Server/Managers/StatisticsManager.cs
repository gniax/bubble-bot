using BubbleBot.Server.Clients;
using BubbleBot.Server.Clients.Accounts;
using BubbleBot.Server.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BubbleBot.Server
{
    public static class StatisticsManager
    {
        public static List<Client> Clients = ServerMain.Clients;
        public static System.Timers.Timer dynamicTimer;

        public static void Initialize()
        {
            dynamicTimer = new System.Timers.Timer();
            dynamicTimer.AutoReset = false;
            dynamicTimer.Elapsed += new System.Timers.ElapsedEventHandler(dynamicTimer_Elapsed);
            dynamicTimer.Interval = GetInterval();
            dynamicTimer.Start();
        }
        public static double GetInterval()
        {
            DateTime now = DateTime.Now;
            return ((300 - now.Second) * 1000 - now.Millisecond);
        }

        public static void dynamicTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            dynamicTimer.Interval = GetInterval();
            if(Clients != null)
            {
                foreach(Client client in Clients)
                {
                    if(client != null && client.Running && client.Network != null && client.Accounts.Count() > 0)
                    {
                        client.SendMessage(new BotsInformationsRequestMessage());
                    }
                }
            }
            dynamicTimer.Start();
        }

    }
}
