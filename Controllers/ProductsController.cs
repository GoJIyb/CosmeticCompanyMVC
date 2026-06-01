using CosmeticCompanyMVC.Interfaces;
using CosmeticCompanyMVC.Models;
using Microsoft.AspNetCore.Mvc;

public class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    private bool IsAdmin()
    {
        return HttpContext.Session.GetString("Role")
            == "Administrator";
    }

    public async Task<IActionResult> Index(string searchString)
    {
        var products =
            string.IsNullOrEmpty(searchString)
            ? await _productService.GetAllProductsAsync()
            : await _productService.SearchProductsAsync(searchString);

        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product =
            await _productService.GetProductAsync(id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    public IActionResult Create()
    {
        if (!IsAdmin())
            return RedirectToAction("Login", "Account");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (!IsAdmin())
            return RedirectToAction("Login", "Account");

        if (ModelState.IsValid)
        {
            await _productService.AddProductAsync(product);

            return RedirectToAction(nameof(Index));
        }

        return View(product);
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!IsAdmin())
            return RedirectToAction("Login", "Account");

        var product =
            await _productService.GetProductAsync(id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        Product product)
    {
        if (!IsAdmin())
            return RedirectToAction("Login", "Account");

        if (id != product.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            await _productService.UpdateProductAsync(product);

            return RedirectToAction(nameof(Index));
        }

        return View(product);
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAdmin())
            return RedirectToAction("Login", "Account");

        var product =
            await _productService.GetProductAsync(id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!IsAdmin())
            return RedirectToAction("Login", "Account");

        await _productService.DeleteProductAsync(id);

        return RedirectToAction(nameof(Index));
    }
}