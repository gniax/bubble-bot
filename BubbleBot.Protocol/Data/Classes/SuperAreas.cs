using Newtonsoft.Json;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Data
{
	public class SuperAreas : IData
	{

		// Properties
		[JsonProperty("id")]
		public int Id { get; set; }
		[JsonProperty("nameId")]
		public string NameId { get; set; }
		[JsonProperty("worldmapId")]
		public int WorldmapId { get; set; }


		//Constructor
		internal SuperAreas() {}

	}
}
