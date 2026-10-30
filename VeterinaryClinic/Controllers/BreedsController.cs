
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Data;
using VeterinaryClinic.Models;

public class BreedsController : Controller
{
    private readonly ApplicationDbContext _context;

    public BreedsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: BREEDS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Breeds
        .Include(breed => breed.AnimalType)
        .ToListAsync());
    }

    // GET: BREEDS/Details/5
    public async Task<IActionResult> Details(int? breedid)
    {
        if (breedid == null)
        {
            return NotFound();
        }

        var breed = await _context.Breeds
            .FirstOrDefaultAsync(m => m.BreedId == breedid);
        if (breed == null)
        {
            return NotFound();
        }

        return View(breed);
    }

    // GET: BREEDS/Create
    public IActionResult Create()
    {
        ViewData["AnimalTypeId"] = new SelectList(
       _context.AnimalTypes,
       "AnimalTypeId",
       "Name");
        return View();
    }

    // POST: BREEDS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BreedId,Name,Description,AnimalTypeId,AnimalType,Animals")] Breed breed)
    {
        if (ModelState.IsValid)
        {
            _context.Add(breed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["AnimalTypeId"] = new SelectList(
       _context.AnimalTypes,
       "AnimalTypeId",
       "Name",
       breed.AnimalTypeId);

        return View(breed);
    }

    // GET: BREEDS/Edit/5
    public async Task<IActionResult> Edit(int? breedid)
    {
        if (breedid == null)
        {
            return NotFound();
        }

        var breed = await _context.Breeds.FindAsync(breedid);
        if (breed == null)
        {
            return NotFound();
        }
        return View(breed);
    }

    // POST: BREEDS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? breedid, [Bind("BreedId,Name,Description,AnimalTypeId,AnimalType,Animals")] Breed breed)
    {
        if (breedid != breed.BreedId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(breed);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BreedExists(breed.BreedId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(breed);
    }

    // GET: BREEDS/Delete/5
    public async Task<IActionResult> Delete(int? breedid)
    {
        if (breedid == null)
        {
            return NotFound();
        }

        var breed = await _context.Breeds
            .FirstOrDefaultAsync(m => m.BreedId == breedid);
        if (breed == null)
        {
            return NotFound();
        }

        return View(breed);
    }

    // POST: BREEDS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? breedid)
    {
        var breed = await _context.Breeds.FindAsync(breedid);
        if (breed != null)
        {
            _context.Breeds.Remove(breed);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BreedExists(int? breedid)
    {
        return _context.Breeds.Any(e => e.BreedId == breedid);
    }
}
