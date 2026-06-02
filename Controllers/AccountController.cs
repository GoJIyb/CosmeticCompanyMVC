using CosmeticCompanyMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace CosmeticCompanyMVC.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<IdentityUser> userManager,
        IEmailSender emailSender,
        ILogger<AccountController> logger)
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
            var userName = string.IsNullOrWhiteSpace(model.UserName)
                ? model.Email
                : model.UserName;

            var user = new IdentityUser
            {
                UserName = userName,
                Email = model.Email,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                try
                {
                    string smtpHost = "smtp.gmail.com";
                    int smtpPort = 587;

                    string smtpUser = "mrx3xxxx@gmail.com";
                    string smtpPass = "wwqd gvsl xcoj vmef";

                    using var client = new SmtpClient(smtpHost, smtpPort)
                    {
                        Credentials = new NetworkCredential(smtpUser, smtpPass),
                        EnableSsl = true
                    };

                    var message = new MailMessage
                    {
                        From = new MailAddress(smtpUser, "CosmeticCompanyMVC"),
                        Subject = "Дані для входу",
                        Body =
            $@"Вітаємо!

Ваш акаунт успішно створено.

Логін: {model.Email}
Пароль: {model.Password}

З повагою,
CosmeticCompanyMVC",
                        IsBodyHtml = false
                    };

                    message.To.Add(model.Email);

                    await client.SendMailAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return RedirectToAction("Login");
            }
            await _userManager.AddToRoleAsync(user, "Member");

            // Надсилаємо email з даними для входу
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
                // Не блокуємо реєстрацію якщо email не надіслано
                _logger.LogWarning(ex,
                    "Не вдалося надіслати лист реєстрації на {Email}", user.Email);
            }

            // Перенаправляємо на сторінку входу
            return RedirectToAction("Login", "Account");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка під час реєстрації");
            ModelState.AddModelError(string.Empty,
                "Сталася помилка при реєстрації. Перегляньте логи сервера.");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByNameAsync(model.Username)
                   ?? await _userManager.FindByEmailAsync(model.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            ViewBag.Error = "Невірний логін або пароль.";
            return View(model);
        }

        // Додайте SignInManager якщо він є в DI; інакше використовується Razor Pages login
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult RegisterConfirmation()
    {
        return View();
    }
}