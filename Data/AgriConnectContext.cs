using AgriEnergyConnect.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;

namespace AgriEnergyConnect.Data
{
    public class AgriConnectContext:IdentityDbContext<DefaultUser> //Add a default user
    {
       public AgriConnectContext(DbContextOptions<AgriConnectContext> options) 
            : base(options) 
        {
        }

        public DbSet<Product> Products { get; set; }
        
        //public DbSet<RegistrationRequest> RegistrationRequests {  get; set; }
    }
}
