using System.ComponentModel.DataAnnotations;

namespace WebLes1Nike.Areas.Admin.Models.Category
{
    public class CategoryEditVM
    {
        public int Id { get; set; }
        [Display(Name = "Назва категорії")]
        [Required(ErrorMessage = "Необхідно вказати назву категорії")]
        public string Name { get; set; } = null!;
        [Display(Name = "Slug (URL)")]
        [Required(ErrorMessage = "Необхідно вказати Slug")]
        public string Slug { get; set; } = null!;
        [Display(Name = "Нове зображення")]
        [DataType(DataType.Upload)]
        public IFormFile? Image { get; set; }
        public string? CurrentImage { get; set; }
    }
}
