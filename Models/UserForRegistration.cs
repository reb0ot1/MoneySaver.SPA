using System.ComponentModel.DataAnnotations;

namespace MoneySaver.SPA.Models
{
    public class UserForRegistration : UserForAuthenticationDto
    {
        public UserForRegistration() { }

        [Compare(nameof(Password))]
        [Required(ErrorMessage = "Confirm password is required.")]
        public string ConfirmPassword { get; set; }
    }
}
