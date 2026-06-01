using CosmeticCompanyMVC.Data;
using CosmeticCompanyMVC.Interfaces;
using CosmeticCompanyMVC.Models;
using CosmeticCompanyMVC.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// База даних
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 1;
    options.User.RequireUniqueEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Сервіси
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// КРОК 1: Міграція БД - окремо перед всім
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        context.Database.Migrate();
        Console.WriteLine("✅ Міграція БД завершена");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Помилка при міграції: {ex.Message}");
    }
}

// КРОК 2: Створення ролей та користувачів - ПІСЛЯ міграції
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    try
    {
        // Створення ролей
        string[] roleNames = { "Administrator", "Member" };
        foreach (var roleName in roleNames)
        {
            bool roleExists = roleManager.RoleExistsAsync(roleName).Result;
            if (!roleExists)
            {
                roleManager.CreateAsync(new IdentityRole(roleName)).Wait();
                Console.WriteLine($"✅ Роль '{roleName}' створена");
            }
        }

        // Створення адміністратора
        var adminUser = userManager.FindByNameAsync("Administrator").Result;
        if (adminUser == null)
        {
            var newAdmin = new IdentityUser
            {
                UserName = "Administrator",
                Email = "admin@cosmeticcompany.com",
                EmailConfirmed = true,
                NormalizedUserName = "ADMINISTRATOR",
                NormalizedEmail = "ADMIN@COSMETICCOMPANY.COM"
            };

            var result = userManager.CreateAsync(newAdmin, "Admin@123").Result;

            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(newAdmin, "Administrator").Wait();
                Console.WriteLine("✅ Адміністратор створен успішно!");
                Console.WriteLine("👤 Username: Administrator");
                Console.WriteLine("🔑 Password: Admin@123");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"❌ Помилка при створенні користувача: {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine("ℹ️ Адміністратор вже існує");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Помилка при ініціалізації: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    }
}

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
