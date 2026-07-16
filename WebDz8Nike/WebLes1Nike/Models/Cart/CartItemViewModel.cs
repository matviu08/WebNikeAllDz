namespace WebLes1Nike.Models.Cart
{
    public class CartItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public string PriceFormatted { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal LineTotal => Price * Quantity;
        public string LineTotalFormatted { get; set; } = null!;
    }
}