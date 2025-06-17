using System.ComponentModel.DataAnnotations;

namespace Zdrav_I_SIlen.Models
{
    public class User
    {
        [Required]
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

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
        public string Password { get; set; } = null!;

        [Display(Name = "Дата на регистрация")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Активен")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Пълно име")]
        public string FullName => $"{FirstName} {LastName}";
    }
} 