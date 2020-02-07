using Newtonsoft.Json;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Data
{
	public class MonsterMiniBoss : IData
	{

		// Properties
		[JsonProperty("id")]
		public int Id { get; set; }
		[JsonProperty("monsterReplacingId")]
		public int MonsterReplacingId { get; set; }


		//Constructor
		internal MonsterMiniBoss() {}

	}
}
