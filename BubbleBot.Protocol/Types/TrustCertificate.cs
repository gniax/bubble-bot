using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class TrustCertificate
	{

		// Properties
		public uint Id { get; set; }
		public string Hash { get; set; }


		// Constructors
		public TrustCertificate() { }

		public TrustCertificate(uint id = 0, string hash = "")
		{
			Id = id;
			Hash = hash;
		}

	}
}
