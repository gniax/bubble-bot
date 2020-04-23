namespace BubbleBot.Protocol.Messages
{
    public class AlmanachCalendarDateMessage : Message
    {

        // Properties
        public int Date { get; set; }


        // Constructors
        public AlmanachCalendarDateMessage() { }

        public AlmanachCalendarDateMessage(int date = 0)
        {
            Date = date;
        }

    }
}
