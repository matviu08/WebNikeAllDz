using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
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
            List<ProductItemViewModel> model = nikeDbContext.Products
                .Select(x => new ProductItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price.ToString("C", new CultureInfo("uk-UA")),
                    CategoryName = x.Category.Name,
                    Images = x.ProductImages
                        .OrderBy(x=> x.Order)
                        .Select(x=>x.Name)
                        .Take(2)
                        .ToList()
                }).ToList();
            return View(model);
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
                string priceStr = model.Price.ToString().Trim().Replace('.', ',');
                decimal price = Decimal.Parse(priceStr, new CultureInfo("uk-UA"));
                var entity = new ProductEntity
                {
                    Name = model.Name,
                    CategoryId = cat.Id,
                    Description = model.Description,
                    Price = price,
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

        public IActionResult Details(int id)
        {
            ProductDetailViewModel? model = nikeDbContext.Products
                .Select(x => new ProductDetailViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price.ToString("C", new CultureInfo("uk-UA")),
                    CategoryName = x.Category.Name,
                    Description = x.Description ?? String.Empty,
                    Images = x.ProductImages
                        .OrderBy(x => x.Order)
                        .Select(x => x.Name)
                        .ToList()
                }).SingleOrDefault(x => x.Id == id);
            return View(model);
        }
    }
}
