using System.ComponentModel.DataAnnotations;

namespace Identity.API.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Email Address is invalid")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
