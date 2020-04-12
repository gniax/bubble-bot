using BubbleBot.Server.Messages;
using BubbleBot.Server.Network;
using BubbleBot.Server.Utility.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace BubbleBot.Server.Utility
{
    class MessageManager
    {
        private static readonly ConcurrentDictionary<Type, List<Action<object>>> _registeredMessages;
        public static void SendMessageToClient(ClientWrapper client, IServerMessage message)
        {
            var bytes = new List<byte>();

            using (BinaryWriter writer = new BinaryWriter(new MemoryStream()))
            {
                writer.Write(message.MessageId);
                message.Serialize(writer);

                bytes.AddRange(writer.GetAllBytes());

                // Insert the message length in the beginning
                bytes.InsertRange(0, BitConverter.GetBytes(bytes.Count));
            }

            client.Send(bytes.ToArray());
        }

        public static void RegisterMessage<T>(Action<T> handler) where T : IServerMessage
        {
            var msgType = typeof(T);
            if (!_registeredMessages.ContainsKey(msgType))
                _registeredMessages.TryAdd(msgType, new List<Action<object>>());

            _registeredMessages[msgType].Add((m) => handler((T)m));
        }
    }
}
