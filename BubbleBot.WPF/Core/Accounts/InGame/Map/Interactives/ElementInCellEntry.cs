namespace BubbleBot.Core.Accounts.InGame.Map.Interactives
{
    public class ElementInCellEntry
    {
        // Constructor
        public ElementInCellEntry(InteractiveElementEntry element, short cellId)
        {
            Element = element;
            CellId = cellId;
        }

        // Properties
        public InteractiveElementEntry Element { get; }
        public short CellId { get; }
    }
}