using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebLes1Nike.Data.Entities.Identity;
using WebLes1Nike.Models;

namespace WebLes1Nike.ViewComponents;

public class UserLinkViewComponent(UserManager<UserEntity> userManager, IConfiguration configuration) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var userName = User.Identity?.Name;
        var model = new UserLinkViewModel();
        if (userName != null)
        {
            var user = userManager.FindByNameAsync(userName).Result;
            var imagesDir = configuration.GetRequiredSection("ImagesDir").Get<string>() ?? "myimages";

            model.Name = $"{user.LastName} {user.FirstName}";
            model.Image = user.Image == "default.jpg"
                ? $"/{imagesDir}/default.jpg"
                : $"/{imagesDir}/{user.Image}_64.webp";
        }
        return View(model);
    }
}