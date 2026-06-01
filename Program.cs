using CosmeticCompanyMVC.Data;
using CosmeticCompanyMVC.Interfaces;
using CosmeticCompanyMVC.Models;
using CosmeticCompanyMVC.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddFilter("Microsoft.AspNetCore.Identity", LogLevel.Debug);

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
    options.User.RequireUniqueEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Ensure DB migrated
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        context.Database.Migrate();
        logger.LogInformation("Database migration applied.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error while migrating database.");
    }
}

// Seed roles and administrator (idempotent)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var roles = new[] { "Administrator", "Member" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var r = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (r.Succeeded)
                {
                    logger.LogInformation("Role '{Role}' created.", roleName);
                }
                else
                {
                    logger.LogWarning("Failed to create role {Role}: {Errors}", roleName, string.Join(';', r.Errors.Select(e => e.Description)));
                }
            }
        }

        // Admin credentials
        var adminUserName = "Administrator";
        var adminEmail = "admin@cosmeticcompany.com";
        var adminPassword = "Admin123";

        // Try find by email first, then by username
        var admin = await userManager.FindByEmailAsync(adminEmail) ?? await userManager.FindByNameAsync(adminUserName);
        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = adminUserName,
                Email = adminEmail,
                EmailConfirmed = true,
                NormalizedUserName = adminUserName.ToUpperInvariant(),
                NormalizedEmail = adminEmail.ToUpperInvariant()
            };

            var createResult = await userManager.CreateAsync(admin, adminPassword);
            if (createResult.Succeeded)
            {
                logger.LogInformation("Administrator user created. Username: {User} Password: {Pwd}", adminUserName, adminPassword);
            }
            else
            {
                logger.LogError("Failed to create admin user: {Errors}", string.Join(';', createResult.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogInformation("Administrator already exists (Id: {Id}, Email: {Email}).", admin.Id, admin.Email);

            // Ensure password is set to known value in dev: reset it
            var token = await userManager.GeneratePasswordResetTokenAsync(admin);
            var resetResult = await userManager.ResetPasswordAsync(admin, token, adminPassword);
            if (resetResult.Succeeded)
            {
                logger.LogInformation("Administrator password reset to '{Pwd}'.", adminPassword);
            }
            else
            {
                logger.LogWarning("Password reset returned errors: {Errors}", string.Join(';', resetResult.Errors.Select(e => e.Description)));
            }
        }

        // Ensure admin is in Administrator role
        if (!await userManager.IsInRoleAsync(admin, "Administrator"))
        {
            var addRoleResult = await userManager.AddToRoleAsync(admin, "Administrator");
            if (addRoleResult.Succeeded)
            {
                logger.LogInformation("Administrator added to role Administrator.");
            }
            else
            {
                logger.LogError("Failed to add Administrator to role: {Errors}", string.Join(';', addRoleResult.Errors.Select(e => e.Description)));
            }
        }
    }
    catch (Exception ex)
    {
        var logger2 = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger2.LogError(ex, "Error while seeding roles/users.");
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
