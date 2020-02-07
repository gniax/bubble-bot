using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BubbleBot.Website.Models
{
    [Table("extensionsbought")]
    public class ExtensionBought
    {

        // Properties
        [Key]
        public int Id { get; set; }
        public int ExtensionId { get; set; }
        public Extension Extension { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int Weeks { get; set; }
        public DateTime Date { get; set; }

        public DateTime EndDate => Date.AddDays(Weeks * 7);


        // Constructors
        public ExtensionBought() { }

        public ExtensionBought(int extensionId, int userId, int weeks)
        {
            ExtensionId = extensionId;
            UserId = userId;
            Weeks = weeks;
            Date = DateTime.Now;
        }

    }
}
