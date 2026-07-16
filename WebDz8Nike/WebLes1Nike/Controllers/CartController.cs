using Microsoft.AspNetCore.Mvc;

namespace WebLes1Nike.Controllers;

public class CartController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}