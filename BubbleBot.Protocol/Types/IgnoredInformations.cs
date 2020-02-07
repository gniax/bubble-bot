using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class IgnoredInformations : AbstractContactInformations
	{

		// Constructors
		public IgnoredInformations() { }

		public IgnoredInformations(uint accountId = 0, string accountName = "")
		{
			AccountId = accountId;
			AccountName = accountName;
		}

	}
}
