using Newtonsoft.Json;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Data
{
	public class NpcActions : IData
	{

		// Properties
		[JsonProperty("id")]
		public int Id { get; set; }
		[JsonProperty("nameId")]
		public string NameId { get; set; }


		//Constructor
		internal NpcActions() {}

	}
}
