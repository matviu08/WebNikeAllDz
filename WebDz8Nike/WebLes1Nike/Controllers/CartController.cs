using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebLes1Nike.Constants;
using WebLes1Nike.Data;
using WebLes1Nike.Extensions;
using WebLes1Nike.Models.Cart;

namespace WebLes1Nike.Controllers;

public class CartController(NikeDbContext nikeDbContext) : Controller
{
    public IActionResult Index()
    {
        var model = HttpContext.Session.GetObject<List<CartItemModel>>(Carts.CartId) ?? [];
        return View(model);
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, int quantity = 1)
    {
        var cart = HttpContext.Session.GetObject<List<CartItemModel>>(Carts.CartId) ?? [];
        var item = cart.FirstOrDefault(x=>x.ProductId == productId);
        if (item != null)
        {
            item.Quantity += quantity;    
        }
        else
        {
            var prod =  nikeDbContext.Products
                .Include(x=>x.ProductImages)
                .Include(x => x.Category)
                .SingleOrDefault(x=>x.Id == productId); 
            item = new CartItemModel
            {
                ProductId = productId,
                Name = prod.Name,
                CategoryName = prod.Category.Name,
                Price = prod.Price,
                Quantity = quantity,
                Image = prod.ProductImages
                    .OrderBy(x=> x.Order)
                    .FirstOrDefault()?.Name ?? "default.jpg"
            };
            cart.Add(item);
        }
        HttpContext.Session.SetObject(Carts.CartId, cart);
        
        return RedirectToAction(nameof(Index));
    }
}