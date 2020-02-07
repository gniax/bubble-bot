using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class ObjectItemNotInContainer : Item
	{

		// Properties
		public List<ObjectEffect> Effects { get; set; }
		public uint ObjectGID { get; set; }
		public uint ObjectUID { get; set; }
		public uint Quantity { get; set; }


		// Constructors
		public ObjectItemNotInContainer() { }

		public ObjectItemNotInContainer(uint objectGID = 0, uint objectUID = 0, uint quantity = 0, List<ObjectEffect> effects = null)
		{
			ObjectGID = objectGID;
			ObjectUID = objectUID;
			Quantity = quantity;
			Effects = effects;
		}

	}
}
