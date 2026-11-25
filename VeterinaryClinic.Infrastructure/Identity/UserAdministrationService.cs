using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using VeterinaryClinic.Application.Users;
using VeterinaryClinic.Infrastructure.Data;

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
    public async Task<DoctorActivationResult> ActivateDoctorAsync(
    string userId,
    string token,
    string password,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(token) ||
            string.IsNullOrWhiteSpace(password))
        {
            return new DoctorActivationResult
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
            return new DoctorActivationResult
            {
                Error = "Поканата е невалидна или вече е използвана."
            };
        }

        // Identity проверява токена, срока му и правилата за паролата.
        var passwordResult = await _userManager.ResetPasswordAsync(
            doctor, token, password);

        if (!passwordResult.Succeeded)
        {
            return new DoctorActivationResult
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
            return new DoctorActivationResult
            {
                Error = "Активирането не завърши. Опитай отново."
            };
        }

        await transaction.CommitAsync(cancellationToken);

        return new DoctorActivationResult
        {
            Succeeded = true
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