using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebLes1Nike.Constants;

namespace WebLes1Nike.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class IconsController : Controller
{
  public IActionResult RiIcons() => View();
}
