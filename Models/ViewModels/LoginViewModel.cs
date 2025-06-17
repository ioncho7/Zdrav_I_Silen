using System.ComponentModel.DataAnnotations;

namespace Zdrav_I_SIlen.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Имейлът е задължителен.")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
        [Display(Name = "Имейл")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Паролата е задължителна.")]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; } = null!;

        [Display(Name = "Запомни ме")]
        public bool RememberMe { get; set; }
    }
} 