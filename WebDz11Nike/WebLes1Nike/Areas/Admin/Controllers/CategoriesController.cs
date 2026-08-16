using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebLes1Nike.Areas.Admin.Models.Category;
using WebLes1Nike.Constants;
using WebLes1Nike.Data;
using WebLes1Nike.Data.Entities;
using WebLes1Nike.Interfaces;

namespace WebLes1Nike.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class CategoriesController(NikeDbContext nikeDbContext, IImageService imageService) : Controller
    {
        public IActionResult Index()
        {
            var model = nikeDbContext.Categories
                .Where(c => !c.isDeleted)
                .Select(c => new CategoryItemVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    Image = c.Image,
                    ProductsCount = c.Products.Count(p => !p.isDeleted)
                })
                .OrderBy(c => c.Name)
                .ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var slugExists = nikeDbContext.Categories
                .Any(c => !c.isDeleted && c.Slug == model.Slug);
            if (slugExists)
            {
                ModelState.AddModelError(nameof(model.Slug), "Категорія з таким Slug вже існує");
                return View(model);
            }

            var category = new CategoryEntitiy
            {
                Name = model.Name,
                Slug = model.Slug,
                Image = "default"
            };

            if (model.Image != null)
            {
                var saveDirectory = Path.Combine(Directory.GetCurrentDirectory(), "myimages");
                category.Image = await imageService.SaveOptimizedImageAsync(model.Image, saveDirectory);
            }

            nikeDbContext.Categories.Add(category);
            await nikeDbContext.SaveChangesAsync();

            TempData["Success"] = $"Категорію «{category.Name}» створено.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = nikeDbContext.Categories.SingleOrDefault(c => c.Id == id && !c.isDeleted);
            if (category == null)
            {
                return NotFound();
            }

            var model = new CategoryEditVM
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                CurrentImage = category.Image
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryEditVM model)
        {
            var category = nikeDbContext.Categories.SingleOrDefault(c => c.Id == model.Id && !c.isDeleted);
            if (category == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                model.CurrentImage = category.Image;
                return View(model);
            }

            var slugTaken = nikeDbContext.Categories
                .Any(c => !c.isDeleted && c.Id != model.Id && c.Slug == model.Slug);
            if (slugTaken)
            {
                ModelState.AddModelError(nameof(model.Slug), "Категорія з таким Slug вже існує");
                model.CurrentImage = category.Image;
                return View(model);
            }

            category.Name = model.Name;
            category.Slug = model.Slug;

            if (model.Image != null)
            {
                var saveDirectory = Path.Combine(Directory.GetCurrentDirectory(), "myimages");
                if (!string.IsNullOrEmpty(category.Image) && category.Image != "default")
                {
                    await imageService.RemoveImageAsync(category.Image, saveDirectory);
                }

                category.Image = await imageService.SaveOptimizedImageAsync(model.Image, saveDirectory);
            }

            await nikeDbContext.SaveChangesAsync();

            TempData["Success"] = $"Категорію «{category.Name}» оновлено.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = nikeDbContext.Categories.SingleOrDefault(c => c.Id == id && !c.isDeleted);
            if (category == null)
            {
                return NotFound();
            }

            var productsCount = nikeDbContext.Products
                .Count(p => p.CategoryId == id && !p.isDeleted);

            if (productsCount > 0)
            {
                TempData["Error"] =
                    $"Неможливо видалити категорію «{category.Name}»: до неї прив'язано {productsCount} товар(ів). " +
                    "Спочатку видаліть ці товари або перенесіть їх в іншу категорію.";
                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrEmpty(category.Image) && category.Image != "default")
            {
                var saveDirectory = Path.Combine(Directory.GetCurrentDirectory(), "myimages");
                await imageService.RemoveImageAsync(category.Image, saveDirectory);
            }

            category.isDeleted = true;
            await nikeDbContext.SaveChangesAsync();

            TempData["Success"] = $"Категорію «{category.Name}» видалено.";
            return RedirectToAction(nameof(Index));
        }
    }
}