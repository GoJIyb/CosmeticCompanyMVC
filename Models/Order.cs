using System.ComponentModel.DataAnnotations;

namespace CosmeticCompanyMVC.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string CustomerName { get; set; }

        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public int Quantity { get; set; }
    }
}