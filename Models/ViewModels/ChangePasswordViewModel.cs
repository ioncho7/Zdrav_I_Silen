using System.ComponentModel.DataAnnotations;

namespace Zdrav_I_SIlen.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Текущата парола е задължителна.")]
        [DataType(DataType.Password)]
        [Display(Name = "Текуща парола")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "Новата парола е задължителна.")]
        [MinLength(6, ErrorMessage = "Паролата трябва да бъде поне 6 символа.")]
        [DataType(DataType.Password)]
        [Display(Name = "Нова парола")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Потвърждението на паролата е задължително.")]
        [Compare("NewPassword", ErrorMessage = "Новите пароли не съвпадат.")]
        [DataType(DataType.Password)]
        [Display(Name = "Потвърди нова парола")]
        public string ConfirmPassword { get; set; } = null!;
    }
} 