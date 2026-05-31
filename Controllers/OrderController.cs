using CosmeticCompanyMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace CosmeticCompanyMVC.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
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
        public IActionResult Create(Product product)
        {
            return View("Result", product);
        }
    }
}
