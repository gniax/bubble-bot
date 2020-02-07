using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class ObjectItemToSellInNpcShop : ObjectItemMinimalInformation
	{

		// Properties
		public uint ObjectPrice { get; set; }
		public string BuyCriterion { get; set; }


		// Constructors
		public ObjectItemToSellInNpcShop() { }

		public ObjectItemToSellInNpcShop(uint objectGID = 0, uint objectPrice = 0, string buyCriterion = "", List<ObjectEffect> effects = null)
		{
			ObjectGID = objectGID;
			ObjectPrice = objectPrice;
			BuyCriterion = buyCriterion;
			Effects = effects;
		}

	}
}
