using System.ComponentModel.DataAnnotations;

namespace Zdrav_I_SIlen.Models.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Името е задължително.")]
        [MaxLength(100, ErrorMessage = "Името не може да надвишава 100 символа.")]
        [Display(Name = "Име")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Имейлът е задължителен.")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
        [Display(Name = "Имейл")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Темата е задължителна.")]
        [MaxLength(200, ErrorMessage = "Темата не може да надвишава 200 символа.")]
        [Display(Name = "Тема")]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "Съобщението е задължително.")]
        [MaxLength(1000, ErrorMessage = "Съобщението не може да надвишава 1000 символа.")]
        [Display(Name = "Съобщение")]
        public string Message { get; set; } = null!;
    }
} 