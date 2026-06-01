using Microsoft.AspNetCore.Identity;
using CosmeticCompanyMVC.Data;

namespace CosmeticCompanyMVC.Data
{
    public class ApplicationUser : IdentityUser
    {
        // Add custom properties here if needed
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}