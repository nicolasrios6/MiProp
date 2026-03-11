using System.ComponentModel.DataAnnotations;

namespace MiProp.Models.ViewModels
{
    public class ResetPasswordVM
    {
        [Required]
        public string Token { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Compare("Password")]
        public string ConfirmPassword { get; set; } = "";
    }
}
