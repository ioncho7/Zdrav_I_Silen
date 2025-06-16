namespace Zdrav_I_SIlen.Models;
using System.ComponentModel.DataAnnotations;


public class Cart
{
    [Required]
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Продуктът трябва да има идентификатор.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Името на продукта е задължително.")]
    [MaxLength(255, ErrorMessage = "Името не може да надвишава 255 символа.")]
    [Display(Name = "Име на продукт")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Цената е задължителна.")]
    [Range(0.01, 100000, ErrorMessage = "Цената трябва да бъде между 0.01 и 100000.")]
    [Display(Name = "Цена")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Моля, въведете количество.")]
    [Range(1, 1000, ErrorMessage = "Количеството трябва да бъде между 1 и 1000.")]
    [Display(Name = "Количество")]
    public int Quantity { get; set; }

    [Display(Name = "Снимка (URL)")]
    [Url(ErrorMessage = "Моля, въведете валиден URL адрес.")]
    [RegularExpression(@".+\.(jpg|jpeg|png|gif|bmp|webp)$",
        ErrorMessage = "URL адресът трябва да завършва с .jpg, .png, .gif и т.н.")]
    public string? Image { get; set; }

    [Display(Name = "Обща стойност")]
    public decimal TotalPrice => Price * Quantity;
} 

