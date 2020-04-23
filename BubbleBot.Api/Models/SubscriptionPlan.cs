using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BubbleBot.Website.Models
{
    [Table("subscriptionsplans")]
    public class SubscriptionPlan
    {

        // Properties
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }

    }
}
