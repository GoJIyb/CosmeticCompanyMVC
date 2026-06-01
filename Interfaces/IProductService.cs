using CosmeticCompanyMVC.Models;

namespace CosmeticCompanyMVC.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();

        Task<Product?> GetProductAsync(int id);

        Task AddProductAsync(Product product);

        Task UpdateProductAsync(Product product);

        Task DeleteProductAsync(int id);

        Task<bool> ProductExistsAsync(int id);

        Task<List<Product>> SearchProductsAsync(string searchString);
    }
}