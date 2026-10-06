using BusinessLogic.BasketItemServices;
using BusinessLogic.BasketServices;
using BusinessLogic.CategoryServices;
using BusinessLogic.CommentServices;
using BusinessLogic.FileUpload;
using BusinessLogic.ProductServices;
using BusinessLogic.ProfileServices;
using DataAccess.Data;
using DataAccess.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// MVC
// =====================================================

builder.Services.AddControllersWithViews();


// =====================================================
// Database
// =====================================================

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "DefaultConnection is not configured.");
}

builder.Services.AddDbContext<GhafarTajhizShopDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});


// =====================================================
// Application Services
// =====================================================

builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<BasketService>();
builder.Services.AddScoped<BasketItemService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();


// =====================================================
// Identity
// =====================================================

builder.Services.AddIdentity<User, Role>(options =>
{
    // Password
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredUniqueChars = 1;

    // Lockout
    options.Lockout.DefaultLockoutTimeSpan =
        TimeSpan.FromMinutes(10);

    options.Lockout.MaxFailedAccessAttempts = 5;

    options.Lockout.AllowedForNewUsers = true;

    // User
    options.User.RequireUniqueEmail = false;
})
.AddEntityFrameworkStores<GhafarTajhizShopDbContext>()
.AddDefaultTokenProviders();


// =====================================================
// Authentication Cookie
// =====================================================

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "GhafarTajhizCustomerCookie";

    options.Cookie.HttpOnly = true;

    options.Cookie.SecurePolicy =
        CookieSecurePolicy.Always;

    options.Cookie.SameSite =
        SameSiteMode.Lax;

    options.ExpireTimeSpan =
        TimeSpan.FromMinutes(60);

    options.SlidingExpiration = true;

    options.LoginPath = "/Account/Login";

    options.AccessDeniedPath =
        "/Account/AccessDenied";
});


// =====================================================
// Build
// =====================================================

var app = builder.Build();


// =====================================================
// HTTP Pipeline
// =====================================================

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


// =====================================================
// Routing
// =====================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();