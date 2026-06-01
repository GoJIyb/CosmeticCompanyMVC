using CosmeticCompanyMVC.Data;
using CosmeticCompanyMVC.Interfaces;
using CosmeticCompanyMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace CosmeticCompanyMVC.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
    }
}