using Newtonsoft.Json;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Data
{
	public class SkinMappings : IData
	{

		// Properties
		[JsonProperty("id")]
		public int Id { get; set; }
		[JsonProperty("lowDefId")]
		public int LowDefId { get; set; }


		//Constructor
		internal SkinMappings() {}

	}
}
