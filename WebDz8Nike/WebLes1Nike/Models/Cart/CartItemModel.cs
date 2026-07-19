namespace WebLes1Nike.Models.Cart;

public class CartItemModel
{
    public int ProductId { get; set; }
    public string Name { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public decimal Price { get; set; }
    public string Image { get; set; } = null!;
    public int Quantity { get; set; }
}