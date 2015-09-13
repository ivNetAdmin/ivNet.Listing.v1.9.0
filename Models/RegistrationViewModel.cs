
using System.ComponentModel.DataAnnotations;

namespace ivNet.Listing.Models
{
    public class RegistrationViewModel
    {
        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        [Display(Name = "Address (line1)")]
        public string Address1 { get; set; }

        [Display(Name = "Address (line2)")]
        public string Address2 { get; set; }

        [Required]
        public string Town { get; set; }

        [Required]
        public string Postcode { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }

        public string Website { get; set; }

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