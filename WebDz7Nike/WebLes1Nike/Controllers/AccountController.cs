using Microsoft.AspNetCore.Mvc;
using WebLes1Nike.Models.Account;

namespace WebLes1Nike.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            
            
            TempData["Message"] = "Форма входу валідна (візуальна демонстрація).";
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            

            TempData["Message"] = "Форма реєстрації валідна (візуальна демонстрація).";
            return RedirectToAction(nameof(Register));
        }
    }
}