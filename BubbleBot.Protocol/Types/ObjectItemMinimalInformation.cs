using System.Collections.Generic;

namespace BubbleBot.Protocol.Types
{
    public class ObjectItemMinimalInformation : Item
    {

        // Properties
        public List<ObjectEffect> Effects { get; set; }
        public uint ObjectGID { get; set; }


        // Constructors
        public ObjectItemMinimalInformation() { }

        public ObjectItemMinimalInformation(uint objectGID = 0, List<ObjectEffect> effects = null)
        {
            ObjectGID = objectGID;
            Effects = effects;
        }

    }
}
