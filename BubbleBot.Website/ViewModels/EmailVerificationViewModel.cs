using System.ComponentModel.DataAnnotations;

namespace BubbleBot.Website.ViewModels
{
    public class EmailVerificationViewModel
    {

        // Properties
        [Required]
        public string ValidationToken { get; set; }
    }
}
