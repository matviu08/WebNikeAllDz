namespace WebLes1Nike.Models.Cart
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new();
        public int TotalCount => Items.Sum(x => x.Quantity);
        public decimal Total => Items.Sum(x => x.LineTotal);
        public string TotalFormatted { get; set; } = null!;
    }
}