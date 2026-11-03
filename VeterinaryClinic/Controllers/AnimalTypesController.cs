
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Data;

[Authorize]
public class AnimalTypesController : Controller
{
    private readonly ApplicationDbContext _context;

    public AnimalTypesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Показване на фиксираните видове животни
    public async Task<IActionResult> Index()
    {
        var animalTypes = await _context.AnimalTypes
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync();

        return View(animalTypes);
    }

    // Подробности за даден вид животно
    public async Task<IActionResult> Details(int? animaltypeid)
    {
        if (animaltypeid == null)
        {
            return NotFound();
        }

        var animalType = await _context.AnimalTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                t => t.AnimalTypeId == animaltypeid);

        if (animalType == null)
        {
            return NotFound();
        }

        return View(animalType);
    }
}
