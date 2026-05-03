using System.ComponentModel.DataAnnotations;

namespace SoundSphereApi.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        [MinLength(3)]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
