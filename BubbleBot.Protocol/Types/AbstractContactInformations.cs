using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class AbstractContactInformations
	{

		// Properties
		public uint AccountId { get; set; }
		public string AccountName { get; set; }


		// Constructors
		public AbstractContactInformations() { }

		public AbstractContactInformations(uint accountId = 0, string accountName = "")
		{
			AccountId = accountId;
			AccountName = accountName;
		}

	}
}
