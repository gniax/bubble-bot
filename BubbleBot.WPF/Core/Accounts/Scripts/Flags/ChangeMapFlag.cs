namespace BubbleBot.Core.Accounts.Scripts.Flags
{
    public class ChangeMapFlag : IFlag
    {
        // Constructor
        public ChangeMapFlag(string where)
        {
            Where = where;
        }

        // Properties
        public string Where { get; }
    }
}