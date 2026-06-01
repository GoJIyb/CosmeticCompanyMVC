using Microsoft.AspNetCore.Mvc;

namespace CosmeticCompanyMVC.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask;

            return View();
        }
    }
}