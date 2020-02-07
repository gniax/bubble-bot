using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class IdentifiedEntityDispositionInformations : EntityDispositionInformations
	{

		// Properties
		public int Id { get; set; }


		// Constructors
		public IdentifiedEntityDispositionInformations() { }

		public IdentifiedEntityDispositionInformations(int cellId = 0, uint direction = 1, int id = 0)
		{
			CellId = cellId;
			Direction = direction;
			Id = id;
		}

	}
}
