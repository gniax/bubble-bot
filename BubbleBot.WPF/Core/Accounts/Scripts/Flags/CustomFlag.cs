using MoonSharp.Interpreter;

namespace BubbleBot.Core.Accounts.Scripts.Flags
{
    public class CustomFlag : IFlag
    {
        // Constructor
        public CustomFlag(DynValue function)
        {
            Function = function;
        }

        // Properties
        public DynValue Function { get; }
    }
}