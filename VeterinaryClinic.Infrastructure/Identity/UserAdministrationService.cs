using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using VeterinaryClinic.Application.Users;
using VeterinaryClinic.Infrastructure.Data;
using System.Data;
using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Application.DTOs;

namespace VeterinaryClinic.Infrastructure.Identity;

public sealed class UserAdministrationService
    : IUserAdministrationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _dbContext;

    public UserAdministrationService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task<CreateDoctorResult> CreateDoctorAsync(
        CreateDoctorRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userName = request.UserName?.Trim() ?? string.Empty;
        var email = request.Email?.Trim() ?? string.Empty;

        if (userName.Length < 3 ||
            userName.Length > 50 ||
            !Regex.IsMatch(userName, @"\A[a-zA-Z0-9._-]+\z"))
        {
            return Failure(
                "Потребителското име трябва да е от 3 до 50 символа: " +
                "латински букви, цифри, точка, тире или долна черта.");
        }

        if (string.IsNullOrWhiteSpace(email) ||
            !new EmailAddressAttribute().IsValid(email))
        {
            return Failure("Въведи валиден имейл.");
        }

        if (!await _roleManager.RoleExistsAsync("Doctor"))
        {
            return Failure(
                "Ролята Doctor липсва. Провери началните роли.");
        }

        if (await _userManager.FindByNameAsync(userName) is not null)
        {
            return Failure("Потребителското име вече е заето.");
        }

        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            return Failure("Вече има профил с този имейл.");
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        var doctor = new ApplicationUser
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = false,
            IsActive = true
        };

        // Създаваме профил без парола.
        // Лекарят ще я зададе чрез защитената покана.
        var createResult = await _userManager.CreateAsync(doctor);

        if (!createResult.Succeeded)
        {
            return Failure(string.Join(
                " ",
                createResult.Errors.Select(error => error.Description)));
        }

        var roleResult = await _userManager.AddToRoleAsync(
            doctor, "Doctor");

        if (!roleResult.Succeeded)
        {
            // Без CommitAsync записите ще бъдат отменени.
            return Failure(
                "Профилът не беше създаден успешно. " +
                "Не успяхме да присвоим ролята Doctor.");
        }

        var token = await _userManager
            .GeneratePasswordResetTokenAsync(doctor);

        await transaction.CommitAsync(cancellationToken);

        return new CreateDoctorResult
        {
            Succeeded = true,
            UserId = doctor.Id,
            PasswordSetupToken = token
        };
    }
    public async Task<UserStatusChangeResult> ActivateDoctorAsync(
    string userId,
    string token,
    string password,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(token) ||
            string.IsNullOrWhiteSpace(password))
        {
            return new UserStatusChangeResult
            {
                Error = "Липсват данни за активиране."
            };
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        var doctor = await _userManager.FindByIdAsync(userId);

        if (doctor is null ||
            !doctor.IsActive ||
            !await _userManager.IsInRoleAsync(doctor, "Doctor") ||
            await _userManager.HasPasswordAsync(doctor))
        {
            return new UserStatusChangeResult
            {
                Error = "Поканата е невалидна или вече е използвана."
            };
        }

        // Identity проверява токена, срока му и правилата за паролата.
        var passwordResult = await _userManager.ResetPasswordAsync(
            doctor, token, password);

        if (!passwordResult.Succeeded)
        {
            return new UserStatusChangeResult
            {
                Error = string.Join(
                    " ",
                    passwordResult.Errors.Select(error => error.Description))
            };
        }

        // Валидната покана разрешава първоначалното активиране.
        doctor.EmailConfirmed = true;

        var updateResult = await _userManager.UpdateAsync(doctor);

        if (!updateResult.Succeeded)
        {
            return new UserStatusChangeResult
            {
                Error = "Активирането не завърши. Опитай отново."
            };
        }

        await transaction.CommitAsync(cancellationToken);

        return new UserStatusChangeResult
        {
            Succeeded = true
        };
    }
    public async Task<IReadOnlyList<UserSummaryDto>> GetUsersAsync(
    CancellationToken cancellationToken = default)
    {
        var users = await _dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.UserName)
            .ToListAsync(cancellationToken);

        var result = new List<UserSummaryDto>();

        foreach (var user in users)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new UserSummaryDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = string.Join(", ", roles.OrderBy(role => role)),
                IsActive = user.IsActive
            });
        }

        return result;
    }

    public async Task<UserStatusChangeResult> SetUserActiveAsync(
        string userId,
        bool isActive,
        string actingAdminId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(actingAdminId))
        {
            return StatusFailure("Липсват данни за операцията.");
        }

        // Защитаваме проверките и промяната като една операция.
        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

        var actingAdmin = await _userManager.FindByIdAsync(
            actingAdminId);

        if (actingAdmin is null ||
            !actingAdmin.IsActive ||
            !await _userManager.IsInRoleAsync(actingAdmin, "Admin"))
        {
            return StatusFailure(
                "Нямаш право да променяш активността на профили.");
        }

        if (!isActive && userId == actingAdminId)
        {
            return StatusFailure(
                "Не можеш да деактивираш собствения си профил.");
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return StatusFailure("Потребителят не е намерен.");
        }

        if (user.IsActive == isActive)
        {
            return new UserStatusChangeResult
            {
                Succeeded = true
            };
        }

        if (!isActive &&
            await _userManager.IsInRoleAsync(user, "Admin"))
        {
            var admins = await _userManager.GetUsersInRoleAsync(
                "Admin");

            if (admins.Count(admin => admin.IsActive) <= 1)
            {
                return StatusFailure(
                    "Последният активен Admin не може да бъде деактивиран.");
            }
        }

        user.IsActive = isActive;

        // Записва променения потребител и обновява security stamp.
        // Старите сесии ще бъдат отхвърлени при проверката му.
        var updateResult = await _userManager
            .UpdateSecurityStampAsync(user);

        if (!updateResult.Succeeded)
        {
            return StatusFailure(
                "Активността на профила не беше променена.");
        }

        await transaction.CommitAsync(cancellationToken);

        return new UserStatusChangeResult
        {
            Succeeded = true
        };
    }

    private static UserStatusChangeResult StatusFailure(string error)
    {
        return new UserStatusChangeResult
        {
            Succeeded = false,
            Error = error
        };
    }
    private static CreateDoctorResult Failure(string error)
    {
        return new CreateDoctorResult
        {
            Succeeded = false,
            Error = error
        };
    }
}