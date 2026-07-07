using Microsoft.AspNetCore.Mvc;
using System.Collections;
using WebLes1Nike.Data;
using WebLes1Nike.Data.Entities;
using WebLes1Nike.Interfaces;
using WebLes1Nike.Models.Category;
using WebLes1Nike.Services;

namespace WebLes1Nike.Controllers;

public class MainController(NikeDbContext nikeDbContext, IImageService imageService) : Controller
{
    //private readonly NikeDbContext _nikeDbContext;
    //public MainController(NikeDbContext nikeDbContext)
    //{
    //    _nikeDbContext = nikeDbContext;
    //}

    public IActionResult Index()
    {
       var list = nikeDbContext.Categories.ToList();

        return View(list);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            CategoryEntitiy categoryEntitiy = new CategoryEntitiy();
            categoryEntitiy.Name = model.Name;
            categoryEntitiy.Slug = model.Slug;
            categoryEntitiy.Image = "default";
            if (model.Image != null)
            {
                var dirName = "images";
                var dirCurrent = Directory.GetCurrentDirectory();
                var saveDirectory = Path.Combine(dirCurrent, "wwwroot", dirName);

                string fileName = Guid.NewGuid().ToString();

                string savedFileName = await imageService.SaveOptimizedImageAsync(model.Image, saveDirectory);

                categoryEntitiy.Image = savedFileName;
            }

            nikeDbContext.Categories.Add(categoryEntitiy);
            nikeDbContext.SaveChanges();

            return Redirect(nameof(Index));
        }
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var cat = nikeDbContext.Categories.SingleOrDefault(x => x.Id == id);
        if (cat == null)
        {
            return NotFound();
        }
        
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        await imageService.RemoveImageAsync(cat.Image, folderPath);

        nikeDbContext.Categories.Remove(cat);
        await nikeDbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var cat = nikeDbContext.Categories.SingleOrDefault(x => x.Id == id);
        if (cat == null)
        {
            return NotFound();
        }

        var model = new CategoryEditViewModel
        {
            Id = cat.Id,
            Name = cat.Name,
            Slug = cat.Slug,
            CurrentImage = cat.Image
        };

        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(CategoryEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var cat = nikeDbContext.Categories.SingleOrDefault(x => x.Id == model.Id);
        if (cat == null)
        {
            return NotFound();
        }
        cat.Name = model.Name;
        cat.Slug = model.Slug;

        if (model.Image != null)
        {
            var dirCurrent = Directory.GetCurrentDirectory();
            var saveDirectory = Path.Combine(dirCurrent, "wwwroot", "images");
            if (!string.IsNullOrEmpty(cat.Image) && cat.Image != "default")
            {
                await imageService.RemoveImageAsync(cat.Image, saveDirectory);
            }
            string savedFileName = await imageService.SaveOptimizedImageAsync(model.Image, saveDirectory);
            cat.Image = savedFileName;
        }

        await nikeDbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
