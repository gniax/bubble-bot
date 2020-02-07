using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeObjectModifiedInBagMessage : ExchangeObjectMessage
	{

		// Properties
		public ObjectItem @Object { get; set; }


		// Constructors
		public ExchangeObjectModifiedInBagMessage() { }

		public ExchangeObjectModifiedInBagMessage(bool remote = false, ObjectItem @object = null)
		{
			Remote = remote;
			@Object = @object;
		}

	}
}
