using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CosmeticCompanyMVC.Models;

namespace CosmeticCompanyMVC.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<AccountController> _logger;

    public AccountController(UserManager<IdentityUser> userManager, IEmailSender emailSender, ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var userName = string.IsNullOrWhiteSpace(model.UserName) ? model.Email : model.UserName;
            var user = new IdentityUser { UserName = userName, Email = model.Email, EmailConfirmed = false };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, "Member");

            var subject = "Ваш акаунт створено";

            var body = $@"
<h3>Реєстрація успішна</h3>

<p>Ваші дані для входу:</p>

<ul>
    <li><strong>Логін:</strong> {user.UserName}</li>
    <li><strong>Email:</strong> {user.Email}</li>
    <li><strong>Пароль:</strong> {model.Password}</li>
</ul>

<p>Збережіть ці дані для подальшого входу.</p>";

            try
            {
                await _emailSender.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send registration email to {Email}", user.Email);
            }

            return RedirectToAction("Login", "Account");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error during registration");
            // Показати загальну помилку на сторінці реєстрації
            ModelState.AddModelError(string.Empty, "Сталася помилка при реєстрації. Перегляньте логи сервера.");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult RegisterConfirmation()
    {
        return View();
    }
}