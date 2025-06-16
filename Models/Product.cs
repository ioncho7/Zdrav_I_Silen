using System.ComponentModel.DataAnnotations;

namespace Zdrav_I_SIlen.Models
{
    public class Product
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Полето 'Име' е задължително.")]
        [MaxLength(255, ErrorMessage = "Името не може да надвишава 255 символа.")]
        [Display(Name = "Име")]
        public string Name { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Описанието не може да надвишава 1000 символа.")]
        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [MaxLength(100, ErrorMessage = "Размерът не може да надвишава 100 символа.")]
        [Display(Name = "Размер")]
        [RegularExpression(@"^([0-9]{1,2}|[A-Z]{1,2}|[0-9]{1,2}кг|[0-9]{1,2}x[0-9]{1,2}|[0-9]{1,2}x[0-9]{1,2}|-)$")]
        public string? Size { get; set; }

        [Display(Name = "Снимка (URL)")]
        [Url(ErrorMessage = "Моля, въведете валиден URL адрес.")]
        [RegularExpression(@"\.(jpg|jpeg|png|gif|bmp|webp|svg)$",
            ErrorMessage = "URL адресът трябва да завършва с разширение на изображение като .jpg, .png и т.н.")]
        public string? ImagePath { get; set; }

        [Range(0.01, 9999), Display(Name = "Цена (лв.)")]
        public decimal UnitPrice { get; set; }

        [Range(0, 1000), Display(Name = "Количество")]
        public int Quantity { get; set; }

        // --- Foreign Key ---
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
