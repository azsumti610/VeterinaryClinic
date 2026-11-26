using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using VeterinaryClinic.Application.Users;
using VeterinaryClinic.Web.ViewModels.AdminUsers;
using System.Security.Claims;

namespace VeterinaryClinic.Web.Controllers;

[Authorize(Roles = "Admin")]
[ResponseCache(
    NoStore = true,
    Location = ResponseCacheLocation.None)]
public class AdminUsersController : Controller
{
    private readonly IUserAdministrationService _users;

    public AdminUsersController(IUserAdministrationService users)
    {
        _users = users;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
     CancellationToken cancellationToken)
    {
        var users = await _users.GetUsersAsync(cancellationToken);

        return View(users);
    }

    [HttpGet]
    public IActionResult CreateDoctor()
    {
        return View(new CreateDoctorViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDoctor(
        CreateDoctorViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _users.CreateDoctorAsync(
            new CreateDoctorRequest
            {
                UserName = model.UserName,
                Email = model.Email
            },
            cancellationToken);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(
                string.Empty,
                result.Error ?? "Неуспешно създаване на лекар.");

            return View(model);
        }

        if (string.IsNullOrWhiteSpace(result.UserId) ||
            string.IsNullOrWhiteSpace(result.PasswordSetupToken))
        {
            throw new InvalidOperationException(
                "Липсват данни за създаване на поканата.");
        }

        var code = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(result.PasswordSetupToken));

        var activationUrl = Url.Action(
            "Activate",
            "DoctorActivation",
            new
            {
                userId = result.UserId,
                code
            },
            protocol: Request.Scheme);

        if (string.IsNullOrWhiteSpace(activationUrl))
        {
            throw new InvalidOperationException(
                "Неуспешно създаване на адрес за поканата.");
        }

        return View("DoctorCreated", new DoctorCreatedViewModel
        {
            UserName = model.UserName.Trim(),
            Email = model.Email.Trim(),
            ActivationUrl = activationUrl
        });

        
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetActive(
    string userId,
    bool isActive,
    CancellationToken cancellationToken)
    {
        var actingAdminId = User.FindFirst(
            ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(actingAdminId))
        {
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest();
        }

        var result = await _users.SetUserActiveAsync(
            userId,
            isActive,
            actingAdminId,
            cancellationToken);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = isActive
                ? "Профилът е активиран."
                : "Профилът е деактивиран.";
        }
        else
        {
            TempData["ErrorMessage"] =
                result.Error ?? "Промяната не беше извършена.";
        }

        return RedirectToAction(nameof(Index));
    }
}