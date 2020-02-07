using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BubbleBot.Server.Commands
{
    public static class CommandsManager
    {

        // Fields
        private static Dictionary<string, MethodInfo> _methods;


        public static void Initialize()
        {
            _methods = new Dictionary<string, MethodInfo>();

            foreach (var type in Assembly.GetEntryAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    if (!method.IsPublic || !method.IsStatic || !method.Name.EndsWith("Command"))
                        continue;

                    if (method.GetCustomAttribute(typeof(CommandAttribute)) is CommandAttribute attr && !_methods.ContainsKey(attr.Command))
                    {
                        _methods.Add(attr.Command, method);
                    }
                }
            }
        }

        public static void HandleCommand(string command)
        {
            // Skip the /
            string[] args = command.Substring(1).Split(' ');

            // args[0] would be the command itself
            command = args[0];

            if (!_methods.ContainsKey(command))
            {
                Console.WriteLine("Commande introuvable.");
                return;
            }

            // Check if there are real arguments
            args = args.Length > 1 ? args.Skip(1).ToArray() : new string[0];

            _methods[command].Invoke(null, new object[] { args });
        }

    }

    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class CommandAttribute : Attribute
    {
        // Properties
        public string Command { get; }


        // Constructor
        public CommandAttribute(string command)
        {
            Command = command;
        }
    }

}