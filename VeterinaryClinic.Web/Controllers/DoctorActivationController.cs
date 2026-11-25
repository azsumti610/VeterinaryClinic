using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using VeterinaryClinic.Application.Users;
using VeterinaryClinic.Web.ViewModels.DoctorActivation;

namespace VeterinaryClinic.Web.Controllers;

[AllowAnonymous]
[ResponseCache(
    NoStore = true,
    Location = ResponseCacheLocation.None)]
public class DoctorActivationController : Controller
{
    private readonly IUserAdministrationService _users;

    public DoctorActivationController(
        IUserAdministrationService users)
    {
        _users = users;
    }

    [HttpGet]
    public IActionResult Activate(string? userId, string? code)
    {
        SetSecurityHeaders();

        if (string.IsNullOrWhiteSpace(userId) ||
            !TryDecodeToken(code, out _))
        {
            return View("InvalidInvitation");
        }

        return View(new ActivateDoctorViewModel
        {
            UserId = userId,
            Code = code!
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(
        ActivateDoctorViewModel model,
        CancellationToken cancellationToken)
    {
        SetSecurityHeaders();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!TryDecodeToken(model.Code, out var token))
        {
            return View("InvalidInvitation");
        }

        var result = await _users.ActivateDoctorAsync(
            model.UserId,
            token,
            model.Password,
            cancellationToken);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(
                string.Empty,
                result.Error ?? "Активирането не завърши.");

            return View(model);
        }

        return RedirectToAction(nameof(Completed));
    }

    [HttpGet]
    public IActionResult Completed()
    {
        SetSecurityHeaders();
        return View();
    }

    private void SetSecurityHeaders()
    {
        Response.Headers["Referrer-Policy"] = "no-referrer";
        Response.Headers["X-Robots-Tag"] = "noindex, nofollow";
    }

    private static bool TryDecodeToken(
        string? code,
        out string token)
    {
        token = string.Empty;

        if (string.IsNullOrWhiteSpace(code) || code.Length > 4096)
        {
            return false;
        }

        try
        {
            token = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(code));

            return !string.IsNullOrWhiteSpace(token);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}