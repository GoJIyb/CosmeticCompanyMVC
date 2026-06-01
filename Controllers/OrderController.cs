using CosmeticCompanyMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticCompanyMVC.Controllers
{
    public class OrderController : Controller
    {
        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask;

            ViewBag.Message = "Ласкаво просимо до оформлення замовлення!";

            Product product = new Product
            {
                Name = "Крем для обличчя",
                Price = 450,
                Description = "Професійний зволожуючий крем"
            };

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            await Task.CompletedTask;

            return View("Result", product);
        }
    }
}