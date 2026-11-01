using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Data;
using VeterinaryClinic.Models;

namespace VeterinaryClinic.Controllers
{
 
    
        [Authorize(Roles = AppRoles.User)]
        public class ProfileController : Controller
        {
            private readonly ApplicationDbContext _context;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly ILogger<ProfileController> _logger;
            public ProfileController(
                ApplicationDbContext context,
                UserManager<ApplicationUser> userManager,
                ILogger<ProfileController> logger)
            {
                _context = context;
                _userManager = userManager;
                _logger = logger;
            }
            [HttpGet]
            public async Task<IActionResult> Index()
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge();
                }
                var owner = await _context.Owners
                    .AsNoTracking()
                    .SingleOrDefaultAsync(
                        owner => owner.ApplicationUserId == user.Id);
                var model = new OwnerProfileViewModel();
                if (owner != null)
                {
                    model.FirstName = owner.FirstName;
                    model.LastName = owner.LastName;
                    model.PhoneNumber = owner.PhoneNumber;
                    model.Email = owner.Email;
                    model.Address = owner.Address;
                }
                else
                {
                    model.Email = user.Email;
                }
                return View(model);
            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Index(
                OwnerProfileViewModel model)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge();
                }
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var owner = await _context.Owners
                    .SingleOrDefaultAsync(
                        owner => owner.ApplicationUserId == user.Id);
                if (owner == null)
                {
                    owner = new Owner
                    {
                        ApplicationUserId = user.Id
                    };
                    _context.Owners.Add(owner);
                }
                owner.FirstName = model.FirstName.Trim();
                owner.LastName = model.LastName.Trim();
                owner.PhoneNumber = model.PhoneNumber?.Trim();
                owner.Email = model.Email?.Trim();
                owner.Address = model.Address?.Trim();
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException exception)
                {
                    _logger.LogError(
                        exception,
                        "Failed to save the owner profile.");
                    ModelState.AddModelError(
                        string.Empty,
                        "Профилът не беше записан. " +
                        "Презаредете страницата и опитайте отново.");
                    return View(model);
                }
                TempData["ProfileSuccess"] =
                    "Профилът е записан успешно.";
                return RedirectToAction(nameof(Index));
            }
        }
    }

