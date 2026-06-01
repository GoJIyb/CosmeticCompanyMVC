using CosmeticCompanyMVC.Models;

namespace CosmeticCompanyMVC.Interfaces
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync();

        Task AddOrderAsync(Order order);
    }
}