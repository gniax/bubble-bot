namespace BubbleBot.Protocol.Messages
{
    public class MountRenamedMessage : Message
    {

        // Properties
        public double MountId { get; set; }
        public string Name { get; set; }


        // Constructors
        public MountRenamedMessage() { }

        public MountRenamedMessage(double mountId = 0, string name = "")
        {
            MountId = mountId;
            Name = name;
        }

    }
}
