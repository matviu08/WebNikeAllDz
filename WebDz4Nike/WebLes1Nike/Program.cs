using Microsoft.EntityFrameworkCore;
using WebHike.Services;
using WebLes1Nike.Data;
using WebLes1Nike.Interfaces;

var builder = WebApplication.CreateBuilder(args);

string strConn = builder.Configuration
    .GetConnectionString("MyWebNikeConnection") ?? "";

builder.Services.AddDbContext<NikeDbContext>(opt =>
    opt.UseNpgsql(strConn));

builder.Services.AddScoped<IImageService, ImageOptimizationService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

var dirName = "images";
var dirCurrent = Directory.GetCurrentDirectory();
var path = Path.Combine(dirCurrent, "wwwroot", dirName);
Directory.CreateDirectory(path);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
