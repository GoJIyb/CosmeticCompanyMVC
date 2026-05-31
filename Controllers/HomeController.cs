
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCompanyMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}