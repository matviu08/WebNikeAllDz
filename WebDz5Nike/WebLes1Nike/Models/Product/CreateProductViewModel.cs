using System.ComponentModel.DataAnnotations;

namespace WebLes1Nike.Models.Product
{
    public class CreateProductViewModel
    {
        [Required(ErrorMessage = "Необхідно вказати назву продукту")]
        [Display(Name = "Назва продукту")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Необхідно вказати опис продукту")]
        [Display(Name = "Опис продукту")]
        public string? Description { get; set; } = String.Empty;
        [Required(ErrorMessage = "Необхідно вказати ціну продукту")]
        [Display(Name = "Ціна продукту")]
        [DataType(DataType.Currency)]
        public string Price { get; set; } = null!;
        [Required(ErrorMessage = "Необхідно вказати URL продукту")]
        [Display(Name = "URL продукту")]
        public string Slug { get; set; } = null!;
        [Required(ErrorMessage = "Необхідно вказати категорію продукту")]
        [Display(Name = "Категорія продукту")]
        public string CategoryName { get; set; } = null!;
        public List<CreateProductImageViewModel> Images { get; set; } = null!;
    }
}
