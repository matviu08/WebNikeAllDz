using Microsoft.AspNetCore.Mvc;
using WebLes1Nike.Data;

namespace WebLes1Nike.Controllers;

public class MainController(NikeDbContext nikeDbContext) : Controller
{
    public IActionResult Index()
    {
        var list = nikeDbContext.Categories
            .Where(c => !c.isDeleted)
            .ToList();

        return View(list);
    }
}