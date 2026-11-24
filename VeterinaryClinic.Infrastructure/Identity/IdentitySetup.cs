using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeterinaryClinic.Infrastructure.Identity;

public static class IdentitySetup
{
    public static async Task EnsureRolesAsync(
    RoleManager<IdentityRole> roleManager)
    {
        string[] roles = ["Admin", "Doctor", "Owner"];
        foreach (var role in roles)
        {
            if (await roleManager.RoleExistsAsync(role))
            {
                continue;
            }
            var result = await roleManager.CreateAsync(
                new IdentityRole(role));
            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(error => error.Description));
                throw new InvalidOperationException(
                    $"Неуспешно създаване на роля '{role}': {errors}");
            }
        }
    }
}