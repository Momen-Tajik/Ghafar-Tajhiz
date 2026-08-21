using BusinessLogic.BasketItemServices;
using BusinessLogic.BasketServices;
using BusinessLogic.CategoryServices;
using BusinessLogic.CommentServices;
using BusinessLogic.FileUpload;
using BusinessLogic.ProductServices;
using BusinessLogic.ProfileServices;
using DataAccess.Data;
using DataAccess.Models;
using DataAccess.Repositories.BasketItemRepo;
using DataAccess.Repositories.BasketRepo;
using DataAccess.Repositories.CategoryRepo;
using DataAccess.Repositories.CommentRepo;
using DataAccess.Repositories.ProductRepo;
using Ghafar_Tajhiz_Admin.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// ===============================
// Database
// ===============================

builder.Services.AddDbContext<GhafarTajhizShopDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});


// ===============================
// Repositories & Services
// ===============================

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<CategoryService>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();

builder.Services.AddScoped<IFileUploadService, FileUploadService>();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddScoped<BasketService>();

builder.Services.AddScoped<IBasketItemRepository, BasketItemRepository>();
builder.Services.AddScoped<BasketItemService>();

builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<CommentService>();

builder.Services.AddScoped<ProfileService>();

builder.Services.AddScoped<UserService>();


// ===============================
// Identity
// ===============================

builder.Services.AddIdentity<User, Role>(options =>
{
    // Password
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
    options.Password.RequiredUniqueChars = 0;

    // Lockout
    options.Lockout.DefaultLockoutTimeSpan =
        TimeSpan.FromMinutes(3);

    options.Lockout.MaxFailedAccessAttempts = 5;

    options.Lockout.AllowedForNewUsers = true;

    // User
    options.User.RequireUniqueEmail = false;
})
.AddEntityFrameworkStores<GhafarTajhizShopDbContext>()
.AddSignInManager<SignInManager<User>>()
.AddDefaultTokenProviders();


// ===============================
// Admin Authentication Cookie
// ===============================

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "GhafarTajhizAdminCookie";

    options.Cookie.HttpOnly = true;

    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

    options.LoginPath = "/AdminAccount/Login";

    options.AccessDeniedPath = "/AdminAccount/AccessDenied";

    options.SlidingExpiration = true;
});


// ===============================
// Build Application
// ===============================

var app = builder.Build();


// ===============================
// HTTP Pipeline
// ===============================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


// ===============================
// MVC Routing
// ===============================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// ===============================
// Seed Roles
// ===============================

using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider
        .GetRequiredService<RoleManager<Role>>();

    var roles = new[]
    {
        "Admin",
        "User"
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(
                new Role
                {
                    Name = role
                });
        }
    }
}


app.Run();