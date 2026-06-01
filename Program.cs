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
<<<<<<< HEAD
    options.Password.RequiredLength = 3;
=======
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 1;
    options.User.RequireUniqueEmail = false;
>>>>>>> 0512fb6d3535aab3391fac82148363b24c45639f
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

<<<<<<< HEAD
// Ініціалізація БД та користувачів
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Міграція БД
        context.Database.Migrate();
        Console.WriteLine("✅ Міграція завершена");

        // Створення ролей
        string[] roleNames = { "Administrator", "Member" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
                Console.WriteLine($"✅ Роль '{roleName}' створена");
            }
=======
// Міграція та ініціалізація ролей та адміністратора
try
{
    var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    // Створення ролей
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Administrator", "Member" };
    
    foreach (var role in roles)
    {
        if (!roleManager.RoleExistsAsync(role).Result)
        {
            roleManager.CreateAsync(new IdentityRole(role)).Wait();
>>>>>>> 0512fb6d3535aab3391fac82148363b24c45639f
        }

        // Створення адміністратора
        string adminEmail = "admin@cosmeticcompany.com";
        string adminPassword = "Admin@123";
        string adminUserName = "Administrator";

        var adminUser = await userManager.FindByNameAsync(adminUserName);
        if (adminUser == null)
        {
            var newAdmin = new IdentityUser
            {
                UserName = adminUserName,
                Email = adminEmail,
                EmailConfirmed = true,
                NormalizedUserName = adminUserName.ToUpper(),
                NormalizedEmail = adminEmail.ToUpper()
            };

            var result = await userManager.CreateAsync(newAdmin, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Administrator");
                Console.WriteLine($"✅ Адміністратор створен: {adminUserName}");
                Console.WriteLine($"📧 Email: {adminEmail}");
                Console.WriteLine($"🔑 Пароль: {adminPassword}");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"❌ Помилка: {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine($"ℹ️ Адміністратор вже існує: {adminUserName}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Помилка при ініціалізації: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
    }

    // Створення адміністратора
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    string adminEmail = "admin@cosmeticcompany.com";
    string adminPassword = "Admin@123"; // ⚠️ ЗМІНІТЬ ЦЕЙ ПАРОЛЬ!

    if (userManager.FindByEmailAsync(adminEmail).Result == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = userManager.CreateAsync(adminUser, adminPassword).Result;
        
        if (result.Succeeded)
        {
            userManager.AddToRoleAsync(adminUser, "Administrator").Wait();
        }
    }

    scope.Dispose();
}
catch (Exception ex)
{
    Console.WriteLine($"Error during initialization: {ex.Message}");
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

<<<<<<< HEAD
app.Run();
=======
app.Run();
>>>>>>> 0512fb6d3535aab3391fac82148363b24c45639f
