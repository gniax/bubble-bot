using BubbleBot.Server.Clients;
using BubbleBot.Server.Messages;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace BubbleBot.Server.Handlers
{
    public static class HandlersManager
    {

        // Fields
        private static Dictionary<string, List<MethodInfo>> _methods;


        static HandlersManager()
        {
            _methods = new Dictionary<string, List<MethodInfo>>();

            bool IsMethodValid(MethodInfo method)
                => method.IsStatic && method.Name.StartsWith("Handle") && method.GetParameters().Length == 2 && method.ReturnType == typeof(Task);

            foreach (var type in typeof(HandlersManager).Assembly.GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    if (!IsMethodValid(method))
                        continue;

                    string msgName = method.Name.Substring(6);

                    if (!_methods.ContainsKey(msgName))
                        _methods.Add(msgName, new List<MethodInfo>());

                    _methods[msgName].Add(method);
                }
            }
        }


        public static void HandleMessage(Client client, IServerMessage message)
        {
            if (message == null)
                return;

            string msgName = message.GetType().Name;

            if (!_methods.ContainsKey(msgName))
                return;

            for (int i = 0; i < _methods[msgName].Count; i++)
            {
                _methods[msgName][i].Invoke(null, new object[] { client, message });
            }
        }

    }
}
