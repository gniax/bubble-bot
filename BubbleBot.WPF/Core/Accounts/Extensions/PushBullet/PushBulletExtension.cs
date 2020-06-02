using BubbleBot.Protocol.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using BubbleBot.Core.Enums;
using ExtensionsEnum = BubbleBot.Protocol.Server.Enums.Extensions;
using BubbleBot.Data;
using BubbleBot.Protocol.Data;
using BubbleBot.Protocol.Messages;
using BubbleBot.Server.Messages;
using BubbleBot.Protocol.Enums;
using BubbleBot.Core.Accounts.InGame.Character.Inventory;
using PushbulletSharp;
using PushbulletSharp.Models.Requests;
using PushbulletSharp.Models.Responses;

namespace BubbleBot.Core.Accounts.Extensions.PushBullet
{
    public class PushBulletExtension : IDisposable
    {
        private Account _account;

        public PushBulletExtension(Account account)
        {
            _account = account;
           // _account.Game.Chat. += Inventory_ObjectEquipped;
        }

        public async Task Initialize()
        {
            PushbulletClient client = new PushbulletClient("o.X4IhyJg0MieC2bvz5PJaf2A0WtdXbtYm");

            //If you don't know your device_iden, you can always query your devices
            var devices = client.CurrentUsersDevices();

            var device = devices.Devices.Where(o => o.Nickname == "OnePlus 6").FirstOrDefault();
            Console.WriteLine(device.Nickname);
            if (device != null)
            {
                PushNoteRequest request = new PushNoteRequest
                {
                    DeviceIden = device.Iden,
                    Title = "hello world",
                    Body = "This is a test from my C# wrapper."
                };

                PushResponse response = client.PushNote(request);
            }

        }
        private void TakeItemToUpdate()
        {
       

        }

        private async Task StartCollect()
        {
     

        }


        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Configuration.Dispose();
                    // _seekChannelTimer.Dispose();
                    //_salesChannelTimer.Dispose();
                    //_generalChannelTimer.Dispose();
                }

                // Configuration = null;
                // _seekChannelTimer = null;
                // _salesChannelTimer = null;
                //_generalChannelTimer = null;
                _account = null;


                disposedValue = true;
            }
        }

        ~PushBulletExtension()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
