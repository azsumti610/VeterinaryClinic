using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ClinicUser = VeterinaryClinic.Infrastructure.Identity.ApplicationUser;

namespace VeterinaryClinic.Web.Identity;

public sealed class ActiveUserSignInManager
    : SignInManager<ClinicUser>
{
    public ActiveUserSignInManager(
        UserManager<ClinicUser> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<ClinicUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<ClinicUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<ClinicUser> confirmation)
        : base(
            userManager,
            contextAccessor,
            claimsFactory,
            optionsAccessor,
            logger,
            schemes,
            confirmation)
    {
    }

    public override async Task<bool> CanSignInAsync(
        ClinicUser user)
    {
        return user.IsActive && await base.CanSignInAsync(user);
    }
}