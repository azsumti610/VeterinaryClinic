using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VeterinaryClinic.Models;

namespace VeterinaryClinic.Data
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = { AppRoles.Doctor, AppRoles.User };
            foreach (var roleName in roles)
            {
                if (await roleManager.RoleExistsAsync(roleName))
                {
                    continue;
                }
                var result = await roleManager.CreateAsync(
                    new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        result.Errors.Select(error => error.Description));
                    throw new InvalidOperationException(
                        $"Неуспешно създаване на роля {roleName}: {errors}");
                }
            }
        }
    }
}
