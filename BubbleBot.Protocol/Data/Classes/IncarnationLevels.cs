using Newtonsoft.Json;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Data
{
	public class IncarnationLevels : IData
	{

		// Properties
		[JsonProperty("id")]
		public int Id { get; set; }
		[JsonProperty("incarnationId")]
		public int IncarnationId { get; set; }
		[JsonProperty("level")]
		public int Level { get; set; }
		[JsonProperty("requiredXp")]
		public int RequiredXp { get; set; }


		//Constructor
		internal IncarnationLevels() {}

	}
}
