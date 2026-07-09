using Microsoft.AspNetCore.Mvc;
using WebLes1Nike.Data;
using WebLes1Nike.Data.Entities;
using WebLes1Nike.Interfaces;
using WebLes1Nike.Models.Product;

namespace WebLes1Nike.Controllers
{
    public class ProductsController(NikeDbContext nikeDbContext, IImageService imageService) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ProdCreate()
        {
            ViewBag.Categories = nikeDbContext
                .Categories.Select(x => x.Name).ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProdCreate(CreateProductViewModel model)
        {
            if ((ModelState.IsValid))
            {
                var cat = nikeDbContext.Categories.SingleOrDefault(x => x.Name == model.CategoryName);
                var entity = new ProductEntity
                {
                    Name = model.Name,
                    CategoryId = cat.Id,
                    Description = model.Description,
                    Price = 0.0M,
                    Slug = model.Slug,
                };
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                var savedImages = await Task.WhenAll(
                    model.Images.Select(async image => new ProductImageEntity
                    {
                        Name = await imageService.SaveOptimizedImageAsync(image.Base64Image, folderPath),
                        Order = image.Order
                    }));
                entity.ProductImages = savedImages.ToList();

                nikeDbContext.Products.Add(entity);
                await nikeDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = nikeDbContext
                .Categories.Select(x => x.Name).ToList();
            return View(model);
        }
    }
}
