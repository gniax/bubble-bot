namespace BubbleBot.Protocol.Messages
{
    public class shopOpenCategoryRequest : Message
    {

        // Properties
        public double CategoryId { get; set; }
        public int Page { get; set; }

        public int Size { get; set; }


        // Constructor
        public shopOpenCategoryRequest(double categoryid, int page, int size)
        {
            CategoryId = categoryid;
            Page = page;
            Size = size;
        }

    }
}

