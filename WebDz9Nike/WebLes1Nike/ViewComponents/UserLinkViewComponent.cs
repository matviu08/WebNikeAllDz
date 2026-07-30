using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebLes1Nike.Data.Entities.Identity;
using WebLes1Nike.Models;

namespace WebLes1Nike.ViewComponents;

public class UserLinkViewComponent(UserManager<UserEntity> userManager) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var userName = User.Identity?.Name;
        var model = new UserLinkViewModel();
        if (userName != null)
        {
            var user = userManager.FindByNameAsync(userName).Result;
            model.Name = $"{user.LastName} {user.FirstName}";
            model.Image = $"/images/{user.Image}_64.webp";
        }
        
        return View(model);
    }
}