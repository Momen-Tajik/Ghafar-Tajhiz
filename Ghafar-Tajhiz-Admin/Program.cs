using BusinessLogic.BasketItemServices;
using BusinessLogic.BasketServices;
using BusinessLogic.CategoryServices;
using BusinessLogic.CommentServices;
using BusinessLogic.FileUpload;
using BusinessLogic.ProductServices;
using BusinessLogic.ProfileServices;
using DataAccess.Data;
using DataAccess.Models;
using Ghafar_Tajhiz_Admin.ExceptionHandling;
using Ghafar_Tajhiz_Admin.Services;
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

var storagePath =
    FileStoragePath.Resolve(
        builder.Environment.ContentRootPath,
        builder.Configuration["FileUpload:StoragePath"]);

Directory.CreateDirectory(storagePath);
Directory.CreateDirectory(Path.Combine(storagePath, "product-images"));
Directory.CreateDirectory(Path.Combine(storagePath, "receipts"));

builder.Services.AddSingleton(
    new FileStorageSettings(storagePath));

builder.Services.AddScoped<IFileUploadService, FileUploadService>();

builder.Services.AddScoped<UserService>();


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
// Admin Authentication Cookie
// =====================================================

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name =
        "GhafarTajhizAdminCookie";

    options.Cookie.HttpOnly = true;

    options.Cookie.SecurePolicy =
        CookieSecurePolicy.SameAsRequest;

    options.Cookie.SameSite =
        SameSiteMode.Lax;

    options.ExpireTimeSpan =
        TimeSpan.FromMinutes(60);

    options.SlidingExpiration = true;

    options.LoginPath =
        "/AdminAccount/Login";

    options.AccessDeniedPath =
        "/AdminAccount/AccessDenied";
});

// =====================================================
// Global Exception Handling
// =====================================================

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

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


// =====================================================
// Seed Roles
// =====================================================

using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<RoleManager<Role>>();

    string[] roles =
    {
        "Admin",
        "User"
    };

    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var result =
                await roleManager.CreateAsync(
                    new Role
                    {
                        Name = roleName
                    });

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Could not create role '{roleName}'.");
            }
        }
    }

    var userManager =
        scope.ServiceProvider
            .GetRequiredService<UserManager<User>>();

    var existingAdmins =
        await userManager.GetUsersInRoleAsync("Admin");

    if (existingAdmins.Count == 0)
    {
        var phoneNumber =
            app.Configuration["AdminSeed:PhoneNumber"]
                ?.Trim();

        var password =
            app.Configuration["AdminSeed:Password"];

        var fullName =
            app.Configuration["AdminSeed:FullName"]
                ?.Trim();

        if (string.IsNullOrWhiteSpace(phoneNumber) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "No admin user exists. Set AdminSeed:PhoneNumber and AdminSeed:Password.");
        }

        var adminUser =
            await userManager.FindByNameAsync(phoneNumber);

        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = phoneNumber,
                PhoneNumber = phoneNumber,
                FullName = string.IsNullOrWhiteSpace(fullName)
                    ? "Admin"
                    : fullName
            };

            var createResult =
                await userManager.CreateAsync(
                    adminUser,
                    password);

            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Could not create the seed admin user: " +
                    string.Join(
                        ", ",
                        createResult.Errors.Select(e => e.Description)));
            }
        }

        var addRoleResult =
            await userManager.AddToRoleAsync(
                adminUser,
                "Admin");

        if (!addRoleResult.Succeeded)
        {
            throw new InvalidOperationException(
                "Could not assign the Admin role to the seed user.");
        }
    }
}


app.Run();