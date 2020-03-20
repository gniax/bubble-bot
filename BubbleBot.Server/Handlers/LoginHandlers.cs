using BubbleBot.Server.Clients;
using BubbleBot.Server.Enums;
using BubbleBot.Server.Messages;
using BubbleBot.Server.Utility;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BubbleBot.Server.Handlers
{
    public static class LoginHandlers
    {

        // Fields
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);


        public static Task HandleLoginRequestMessage(Client client, LoginRequestMessage message)
            => Task.Run(async () =>
            {
                await _semaphore.WaitAsync();

                var response = await HttpClientUtility.GetJsonAsync($"login?username={message.Username}&password={message.Password}&token=1997");

                // In case something goes wrong
                if (response == null)
                {
                    _semaphore.Release();
                    return;
                }

                // Login succeeded
                if (response.Value<bool>("success"))
                {
                    client.Informations.Set(response, message.Password);

                    // Check if this user has reached his maximum instances count
                    if (ServerMain.GetClientInstancesCount(client.Informations.Name) >= client.Informations.MaxInstances)
                    {
                        client.Informations.SetDefault();
                        client.SendMessage(new LoginRefusedMessage((byte)LoginResults.TOO_MANY_INSTANCES));
                    }
                    else
                    {
                        client.LoggedIn = true;
                        client.SendMessage(new LoginAcceptedMessage(client.Informations.Name, response["user"].Value<string>("avatar")));
                        //Console.WriteLine("AppV: {0} BuildV: {1}, AssetsV: {2}, StaticDataV: {3}", Constants.AppVersion, Constants.BuildVersion, Constants.AssetsVersion, Constants.StaticDataVersion);
                        client.SendMessage(new DTVersionsMessage(Constants.AppVersion, Constants.BuildVersion, Constants.AssetsVersion, Constants.StaticDataVersion));

                        // Send a statistics message
                        ServerMain.BroadcastStatistics();

                        // Only send this if the user is subscribed
                        if (client.Informations.IsSubscribedToTouch)
                        {
                            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                            client.SendMessage(new SubscriptionInformationsMessage(client.Informations.TouchEndDate, client.Informations.Extensions));
                        }

                        client.Informations.StartRefreshing();
                    }
                }
                // Login failed
                else
                {
                    byte messageId = response.Value<byte>("errorId");
                    Console.WriteLine("Client failed to login (reason: {0}).", (LoginResults)messageId);
                    client.SendMessage(new LoginRefusedMessage(messageId));
                }

                _semaphore.Release();
            });

    }
}
