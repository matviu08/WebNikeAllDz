namespace WebLes1Nike.Areas.Admin.Models.Category;

public class CategoryItemVM
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Image { get; set; }
    public int ProductsCount { get; set; }
}