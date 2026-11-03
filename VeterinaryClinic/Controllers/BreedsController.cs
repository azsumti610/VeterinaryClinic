
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Data;

[Authorize]
public class BreedsController : Controller
{
    private readonly ApplicationDbContext _context;

    public BreedsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var breeds = await _context.Breeds
            .Include(b => b.AnimalType)
            .AsNoTracking()
            .OrderBy(b => b.AnimalType!.Name)
            .ThenBy(b => b.Name)
            .ToListAsync();

        return View(breeds);
    }

    public async Task<IActionResult> Details(int? breedid)
    {
        if (breedid == null)
            return NotFound();

        var breed = await _context.Breeds
            .Include(b => b.AnimalType)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BreedId == breedid);

        if (breed == null)
            return NotFound();

        return View(breed);
    }

    // Показва формата за редактиране на описание
    [HttpGet]
    public async Task<IActionResult> Edit(int? breedid)
    {
        if (breedid == null)
            return NotFound();

        var breed = await _context.Breeds.FindAsync(breedid);

        if (breed == null)
            return NotFound();

        return View(breed);
    }

    // Запазва само описанието
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int breedid,
        string? description)
    {
        var breed = await _context.Breeds.FindAsync(breedid);

        if (breed == null)
            return NotFound();

        if (description?.Length > 1000)
        {
            ModelState.AddModelError(
                "Description",
                "Описанието не може да надвишава 1000 символа.");

            return View(breed);
        }

        breed.Description = description;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
