using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BubbleBot.Website.Models
{
    [Table("pointsplans")]
    public class PointsPlan
    {

        // Properties
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public double Price { get; set; }

        public List<PaypalTransaction> PaypalTransactions { get; set; }

        public string FormattedPrice => string.Format("{0:0.00}", Price).Replace(',', '.');

    }
}
