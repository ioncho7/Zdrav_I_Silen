using System.ComponentModel.DataAnnotations;

namespace Zdrav_I_SIlen.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Името е задължително.")]
        [MaxLength(50, ErrorMessage = "Името не може да надвишава 50 символа.")]
        [Display(Name = "Име")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Фамилията е задължителна.")]
        [MaxLength(50, ErrorMessage = "Фамилията не може да надвишава 50 символа.")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Имейлът е задължителен.")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
        [Display(Name = "Имейл")]
        public string Email { get; set; } = null!;

        [Phone(ErrorMessage = "Невалиден телефонен номер.")]
        [Display(Name = "Телефон")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Паролата е задължителна.")]
        [MinLength(6, ErrorMessage = "Паролата трябва да бъде поне 6 символа.")]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Потвърждението на паролата е задължително.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролите не съвпадат.")]
        [Display(Name = "Потвърди парола")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "Трябва да се съгласите с общите условия.")]
        public bool AgreeToTerms { get; set; }
    }
} 