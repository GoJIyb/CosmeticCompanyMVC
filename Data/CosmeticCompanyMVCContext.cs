using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class CosmeticCompanyMVCContext(DbContextOptions<CosmeticCompanyMVCContext> options) : IdentityDbContext<CosmeticCompanyMVC.Data.IdentityUser>(options)
{
}
