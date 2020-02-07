using System.ComponentModel.DataAnnotations;

namespace BubbleBot.Website.ViewModels
{
    public class RegisterViewModel
    {

        // Properties
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

    }
}
