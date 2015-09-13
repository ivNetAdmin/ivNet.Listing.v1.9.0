
using System.ComponentModel.DataAnnotations;

namespace ivNet.Listing.Models
{
    public class ActivationViewModel
    {
        public string Message { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirm")]
        public string ConfirmPassword { get; set; }
    }
}