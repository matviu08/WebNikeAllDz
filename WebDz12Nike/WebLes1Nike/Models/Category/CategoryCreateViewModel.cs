using System.ComponentModel.DataAnnotations;

namespace WebLes1Nike.Models.Category
{
    public class CategoryCreateViewModel
    {
        [Display(Name = "Назва категорії")]
        [Required(ErrorMessage = "Необхідно вказати назву категорії")]
        public string Name { get; set; } = null!;
        [Display(Name = "Slug (URL)")]
        [Required(ErrorMessage = "Необхідно вказати Slug")]
        public string Slug { get; set; } = null!;
        [Display(Name = "Зображення")]
        [DataType(DataType.Upload)]
        public IFormFile? Image { get; set; } = null!;
    }
}
