using Microsoft.AspNetCore.Mvc;
using WebLes1Nike.Constants;
using WebLes1Nike.Extensions;
using WebLes1Nike.Models.Cart;

namespace WebLes1Nike.ViewComponents;

public class CartCounterViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var cart = HttpContext.Session.GetObject<List<CartItemModel>>(Carts.CartId) ?? [];
        var count = cart.Sum(x => x.Quantity);

        return View(count);
    }
}