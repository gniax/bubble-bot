using System;

namespace BubbleBot.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        // Constructor
        public CommandAttribute(string command)
        {
            Command = command;
        }

        // Properties
        public string Command { get; set; }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class RemainerAttribute : Attribute
    {
    }
}