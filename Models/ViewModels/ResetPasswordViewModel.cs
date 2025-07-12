using System.ComponentModel.DataAnnotations;

namespace Zdrav_I_SIlen.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Имейлът е задължителен.")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
        [Display(Name = "Имейл")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Кодът за възстановяване е задължителен.")]
        [Display(Name = "Код за възстановяване")]
        public string ResetCode { get; set; } = null!;

        [Required(ErrorMessage = "Новата парола е задължителна.")]
        [MinLength(6, ErrorMessage = "Паролата трябва да бъде поне 6 символа.")]
        [DataType(DataType.Password)]
        [Display(Name = "Нова парола")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Потвърждението на паролата е задължително.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Паролите не съвпадат.")]
        [Display(Name = "Потвърди новата парола")]
        public string ConfirmPassword { get; set; } = null!;
    }
} 