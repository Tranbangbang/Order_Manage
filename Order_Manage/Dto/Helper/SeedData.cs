using Microsoft.AspNetCore.Identity;
using Order_Manage.Models;

namespace Order_Manage.Dto.Helper
{
    public static class SeedData
    {
        public static async Task InitializeRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] { UserRoles.Admin,UserRoles.User};
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }


        public static async Task InitializeAdminAccount(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<Account>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            var adminEmail = configuration["DefaultAdmin:Email"];
            var adminPassword = configuration["DefaultAdmin:Password"];

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminAccount = new Account
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    AccountName = "Default Admin"
                };

                var result = await userManager.CreateAsync(adminAccount, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminAccount, "Admin");
                }
            }
        }

    }
}
