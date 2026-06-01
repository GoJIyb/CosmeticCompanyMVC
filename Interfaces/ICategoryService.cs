using CosmeticCompanyMVC.Models;

namespace CosmeticCompanyMVC.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoriesAsync();
    }
}