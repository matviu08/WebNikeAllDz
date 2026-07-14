using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebHike.Services;
using WebLes1Nike.Constants;
using WebLes1Nike.Data;
using WebLes1Nike.Data.Entities.Identity;
using WebLes1Nike.Interfaces;

var builder = WebApplication.CreateBuilder(args);

string strConn = builder.Configuration
    .GetConnectionString("MyWebNikeConnection") ?? "";

builder.Services.AddDbContext<NikeDbContext>(opt =>
    opt.UseNpgsql(strConn));

builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;
    })
    .AddEntityFrameworkStores<NikeDbContext>()
    .AddDefaultTokenProviders();

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManger = services.GetRequiredService<RoleManager<RoleEntity>>();
    var userManger = services.GetRequiredService<UserManager<UserEntity>>();
    
    if (!roleManger.Roles.Any())
    {
        foreach (var roleName in Roles.ListRoles())
        {
            await roleManger.CreateAsync(new RoleEntity { Name = roleName });
        }
    }

    if (!userManger.Users.Any())
    {
        var user = new UserEntity
        {
            Email = "admin@gmail.com",
            UserName = "admin@gmail.com",
            FirstName = "Admin",
            LastName = "Admincuk",
            Image = "default.jpg"
        };
        await userManger.CreateAsync(user, "Qwerty1-");
        await userManger.AddToRoleAsync(user, Roles.Admin);
    }
}

app.Run();
