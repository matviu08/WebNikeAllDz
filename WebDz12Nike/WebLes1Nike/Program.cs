using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
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

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(30);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseSession();

//  var dirName = "images";
// var dirCurrent = Directory.GetCurrentDirectory();
// var path = Path.Combine(dirCurrent, "wwwroot", dirName);
// Directory.CreateDirectory(path);

try
{
    Console.WriteLine("myImages "+ builder.Configuration.GetRequiredSection("imagesDir").Get<string>() ?? "myimages");
    string imagesDir = builder.Configuration.GetRequiredSection("ImagesDir").Get<string>() ?? "myimages";
    string path = Path.Combine(Directory.GetCurrentDirectory(), imagesDir);
    Directory.CreateDirectory(path);

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(path),
        RequestPath = $"/{imagesDir}"
    });
}
catch(Exception ex)
{
    Console.WriteLine("Помилка запуску" + ex.Message);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Main}/{action=Index}/{id?}")
//     .WithStaticAssets();

app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name: "admin_area",
        areaName: "Admin",
        pattern: "admin/{controller=Dashboards}/{action=Index}/{id?}"
    );
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManger = services.GetRequiredService<RoleManager<RoleEntity>>();
    var userManger = services.GetRequiredService<UserManager<UserEntity>>();
    var dbContext = services.GetRequiredService<NikeDbContext>();
    
    await dbContext.Database.MigrateAsync();
    
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
