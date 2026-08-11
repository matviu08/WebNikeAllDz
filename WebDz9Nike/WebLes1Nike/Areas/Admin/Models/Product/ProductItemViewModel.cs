namespace WebLes1Nike.Areas.Admin.Models.Product;

public class ProductItemVM
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Price { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public List<string> Images { get; set; } = null!;

}
