using CosmeticCompanyMVC.Data;
using CosmeticCompanyMVC.Interfaces;
using CosmeticCompanyMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace CosmeticCompanyMVC.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task AddOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }
    }
}