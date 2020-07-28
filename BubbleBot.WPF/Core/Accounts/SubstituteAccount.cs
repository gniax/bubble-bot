using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.Configurations;
using BubbleBot.Core.Accounts.Extensions;
using BubbleBot.Core.Accounts.InGame;
using BubbleBot.Core.Accounts.Network;
using BubbleBot.Core.Accounts.Scripts;
using BubbleBot.Core.Accounts.Statistics;
using BubbleBot.Core.Commands;
using BubbleBot.Core.Enums;
using BubbleBot.Core.Groups;
using BubbleBot.Core.Logs;
using BubbleBot.Data;
using BubbleBot.Protocol.Messages;
using BubbleBot.Utility;
using BubbleBot.Utility.Security;
using CefSharp;
using CefSharp.OffScreen;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace BubbleBot.Core.Accounts
{
    public class SubstituteAccount : ViewModelBase
    {
        public string Username { get; set; }

        [JsonConverter(typeof(EncryptingJsonConverter), "Bûbbl€Bôt")]
        public string Password { get; set; }

        public SubstituteAccount(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
