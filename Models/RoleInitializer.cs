using Microsoft.AspNetCore.Identity;

namespace AgriEnergyConnect.Models
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<DefaultUser>>();

            string[] roleNames = { "Employee", "Farmer" };

            IdentityResult roleResult;

            foreach (var role in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var email = "admin@site.com";
            var password = "Qwerty123!";

            if (userManager.FindByEmailAsync(email).Result == null)
            {
                DefaultUser user = new()
                {
                    Email = email,
                    UserName = email,
                    FirstName = "Admin",
                    LastName = "Adminsson",
                    Address = "Adstreet 3",
                    City = "Big City",
                };

                IdentityResult result = userManager.CreateAsync(user, password).Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Employee").Wait();
                }
            }

        }
    }
}
