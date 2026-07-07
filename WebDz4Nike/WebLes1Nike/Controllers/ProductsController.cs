using Microsoft.AspNetCore.Mvc;
using WebLes1Nike.Data;

namespace WebLes1Nike.Controllers
{
    public class ProductsController(NikeDbContext nikeDbContext) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = nikeDbContext
                .Categories.Select(x => x.Name).ToList();
            return View();
        }
    }
}
