using CosmeticCompanyMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCompanyMVC.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (model.Username == "admin" &&
                model.Password == "admin")
            {
                HttpContext.Session.SetString(
                    "Role",
                    "Administrator");

                return RedirectToAction(
                    "Index",
                    "Home");
            }

            if (model.Username == "member" &&
                model.Password == "member")
            {
                HttpContext.Session.SetString(
                    "Role",
                    "Member");

                return RedirectToAction(
                    "Index",
                    "Home");
            }

            ViewBag.Error =
                "Невірний логін або пароль";

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction(
                "Login");
        }
    }
}