using GalaSoft.MvvmLight;

namespace BubbleBot.Core.Accounts.Statistics
{
    public class ObjectObtainedEntry : ViewModelBase
    {
        private uint _percentage;

        // Fields
        private uint _quantity;

        // Constructor
        public ObjectObtainedEntry(uint gid, string name, uint qty)
        {
            GID = gid;
            Name = name;
            Quantity = qty;
        }


        // Properties
        public uint GID { get; }
        public string Name { get; }

        public uint Quantity
        {
            get => _quantity;
            set => Set(ref _quantity, value);
        }

        public uint Percentage
        {
            get => _percentage;
            set => Set(ref _percentage, value);
        }
    }
}