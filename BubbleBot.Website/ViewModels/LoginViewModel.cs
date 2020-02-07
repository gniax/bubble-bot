using System.ComponentModel.DataAnnotations;

namespace BubbleBot.Website.ViewModels
{
    public class LoginViewModel
    {

        // Properties
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
