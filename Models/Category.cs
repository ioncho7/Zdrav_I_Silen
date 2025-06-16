namespace Zdrav_I_SIlen.Models;
using System.ComponentModel.DataAnnotations;
    public class Category
    {
        [Required]
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Името на категорията е задължително.")]
        [MaxLength(30, ErrorMessage = "Името на категорията не може да надвишава 30 символа.")]
        [Display(Name = "Име на категория")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Номерацията е задължителна.")]
        [Range(1, 100, ErrorMessage = "Номерацията трябва да бъде между 1 и 100.")]
        [Display(Name = "Номерация")]
        public int DisplayOrder { get; set; }
    }
