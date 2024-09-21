using Dashboard.DAL.Models.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Dashboard.DAL.Data.Initializer
{
    public static class DataSeeder
    {
        public const string AdminRole = "Administrator";
        public const string UserRole = "User";

        public static async void SeedData(this IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

                // Створення ролей
                if (!await roleManager.RoleExistsAsync(AdminRole))
                {
                    await roleManager.CreateAsync(new Role { Name = AdminRole });
                }
                if (!await roleManager.RoleExistsAsync(UserRole))
                {
                    await roleManager.CreateAsync(new Role { Name = UserRole });
                }

                // Створення адміністратора
                var adminEmail = "admin@example.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new User
                    {
                        UserName = "admin",
                        Email = adminEmail,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(adminUser, "AdminPassword123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, AdminRole);
                    }
                }

                // Створення користувачів
                for (int i = 1; i <= 3; i++)
                {
                    var userEmail = $"user{i}@example.com";
                    var user = await userManager.FindByEmailAsync(userEmail);
                    if (user == null)
                    {
                        user = new User
                        {
                            UserName = $"user{i}",
                            Email = userEmail,
                            EmailConfirmed = true
                        };
                        var result = await userManager.CreateAsync(user, $"UserPassword{i}!");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, UserRole);
                        }
                    }
                }
            }
        }
    }
}
