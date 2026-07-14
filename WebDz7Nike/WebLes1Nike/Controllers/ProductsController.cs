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
        [HttpGet]
        public IActionResult ProdEdit(int id)
        {
            var product = nikeDbContext.Products
                .Include(x => x.ProductImages)
                .Include(x => x.Category)
                .SingleOrDefault(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price.ToString(CultureInfo.InvariantCulture),
                Slug = product.Slug,
                CategoryName = product.Category.Name,
                ExistingImages = product.ProductImages
                    .OrderBy(x => x.Order)
                    .Select(x => new ExistingProductImageViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Order = x.Order
                    }).ToList()
            };

            ViewBag.Categories = nikeDbContext.Categories.Select(x => x.Name).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProdEdit(ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = nikeDbContext.Categories.Select(x => x.Name).ToList();
                return View(model);
            }
            var product = nikeDbContext.Products
                .Include(x => x.ProductImages)
                .SingleOrDefault(x => x.Id == model.Id);
            if (product == null)
            {
                return NotFound();
            }
            var cat = nikeDbContext.Categories.SingleOrDefault(x => x.Name == model.CategoryName);
            if (cat == null)
            {
                ModelState.AddModelError(nameof(model.CategoryName), "Обрана категорія не знайдена");
                ViewBag.Categories = nikeDbContext.Categories.Select(x => x.Name).ToList();
                return View(model);
            }
            string priceStr = model.Price.ToString().Trim().Replace('.', ',');
            decimal price = Decimal.Parse(priceStr, new CultureInfo("uk-UA"));
            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = price;
            product.Slug = model.Slug;
            product.CategoryId = cat.Id;

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            var keptIds = model.ExistingImages.Select(x => x.Id).ToHashSet();
            var imagesToRemove = product.ProductImages.Where(x => !keptIds.Contains(x.Id)).ToList();
            foreach (var img in imagesToRemove)
            {
                await imageService.RemoveImageAsync(img.Name, folderPath);
                nikeDbContext.ProductImages.Remove(img);
            }

            foreach (var existing in model.ExistingImages)
            {
                var img = product.ProductImages.SingleOrDefault(x => x.Id == existing.Id);
                if (img != null)
                {
                    img.Order = existing.Order;
                }
            }

            if (model.NewImages != null && model.NewImages.Count > 0)
            {
                var savedImages = await Task.WhenAll(
                    model.NewImages.Select(async image => new ProductImageEntity
                    {
                        Name = await imageService.SaveOptimizedImageAsync(image.Base64Image, folderPath),
                        Order = image.Order,
                        ProductId = product.Id
                    }));

                foreach (var img in savedImages)
                {
                    nikeDbContext.ProductImages.Add(img);
                }
            }
            await nikeDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ProdDelete(int id)
        {
            var product = nikeDbContext.Products
                .Include(x => x.ProductImages)
                .SingleOrDefault(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            foreach (var img in product.ProductImages)
            {
                await imageService.RemoveImageAsync(img.Name, folderPath);
            }
            nikeDbContext.ProductImages.RemoveRange(product.ProductImages);
            nikeDbContext.Products.Remove(product);
            await nikeDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
