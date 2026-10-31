
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Models;
using VeterinaryClinic.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

public class AnimalsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AnimalsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: ANIMALS
    public async Task<IActionResult> Index()    
    {
        var animals = await _context.Animals
       .Include(animal => animal.Breed)
       .Include(animal => animal.Owner)
       .ToListAsync();
        return View(animals);
    }

    // GET: ANIMALS/Details/5
    public async Task<IActionResult> Details(int? animalid)
    {
        if (animalid == null)
        {
            return NotFound();
        }

        var animal = await _context.Animals
            .FirstOrDefaultAsync(m => m.AnimalId == animalid);
        if (animal == null)
        {
            return NotFound();
        }

        return View(animal);
    }

    // GET: ANIMALS/Create
    public IActionResult Create()
    {
        ViewData["BreedId"] = new SelectList(
        _context.Breeds,
        "BreedId",
        "Name");
        ViewData["OwnerId"] = new SelectList(
            _context.Owners.Select(owner => new
            {
                owner.OwnerId,
                FullName = owner.FirstName + " " + owner.LastName
            }),
            "OwnerId",
            "FullName");

        return View();
    }

    // POST: ANIMALS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AnimalId,Name,BreedId,OwnerId,BirthDate,Description,ImagePath")] Animal animal)
    {
        if (ModelState.IsValid)
        {
            _context.Add(animal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["BreedId"] = new SelectList(
       _context.Breeds,
       "BreedId",
       "Name",
       animal.BreedId);
        ViewData["OwnerId"] = new SelectList(
            _context.Owners.Select(owner => new
            {
                owner.OwnerId,
                FullName = owner.FirstName + " " + owner.LastName
            }),
            "OwnerId",
            "FullName",
            animal.OwnerId);

        return View(animal);
    }

    // GET: ANIMALS/Edit/5
    public async Task<IActionResult> Edit(int? animalid)
    {
        if (animalid == null)
        {
            return NotFound();
        }

        var animal = await _context.Animals.FindAsync(animalid);
        if (animal == null)
        {
            return NotFound();
        }
        return View(animal);
    }

    // POST: ANIMALS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? animalid, [Bind("AnimalId,Name,BreedId,Breed,OwnerId,Owner,BirthDate,Description,ImagePath,Examinations")] Animal animal)
    {
        if (animalid != animal.AnimalId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(animal);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalExists(animal.AnimalId))
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
        return View(animal);
    }

    // GET: ANIMALS/Delete/5
    public async Task<IActionResult> Delete(int? animalid)
    {
        if (animalid == null)
        {
            return NotFound();
        }

        var animal = await _context.Animals
            .FirstOrDefaultAsync(m => m.AnimalId == animalid);
        if (animal == null)
        {
            return NotFound();
        }

        return View(animal);
    }

    // POST: ANIMALS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? animalid)
    {
        var animal = await _context.Animals.FindAsync(animalid);
        if (animal != null)
        {
            _context.Animals.Remove(animal);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AnimalExists(int? animalid)
    {
        return _context.Animals.Any(e => e.AnimalId == animalid);
    }
}
