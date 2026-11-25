using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeterinaryClinic.Infrastructure.Identity
{
    public static class InitialAdminSetup
    {
        public static async Task EnsureAsync(
        UserManager<ApplicationUser> userManager,
        bool enabled,
        string? email,
        string? password)
        {
            if (!enabled)
            {
                return;
            }
            var admins = await userManager.GetUsersInRoleAsync("Admin");
            if (admins.Count > 0)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                password.Length < 12)
            {
                throw new InvalidOperationException(
                    "Провери InitialAdmin в User Secrets. " +
                    "Нужни са имейл и парола с поне 12 символа.");
            }
            email = email.Trim();
            if (await userManager.FindByEmailAsync(email) is not null)
            {
                throw new InvalidOperationException(
                    "Вече има потребител с този имейл. " +
                    "Началната настройка няма да променя ролята му.");
            }
            var admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                IsActive = true
            };
            var createResult = await userManager.CreateAsync(admin, password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Admin не е създаден. Провери имейла " +
                    "и изискванията на Identity за паролата.");
            }
            var roleResult = await userManager.AddToRoleAsync(admin, "Admin");
            if (!roleResult.Succeeded)
            {
                var cleanupResult = await userManager.DeleteAsync(admin);
                throw new InvalidOperationException(cleanupResult.Succeeded
                    ? "Ролята Admin не беше присвоена. Новият профил е премахнат."
                    : "Ролята Admin не беше присвоена и профилът не беше премахнат. " +
                      "Нужна е проверка преди нов опит.");
            }
        }
    }
}
