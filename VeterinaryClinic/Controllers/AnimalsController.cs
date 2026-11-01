
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Models;
using VeterinaryClinic.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;

public class AnimalsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AnimalsController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
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
        PopulateDropdowns();
        return View();
    }

    // POST: ANIMALS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AnimalId,Name,BreedId,OwnerId,BirthDate,Description,ImageFile")] Animal animal)
    {
        var imageFile = animal.ImageFile;
        string? extension = null;
        if (imageFile is { Length: > 0 })
        {
            extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(
                    nameof(animal.ImageFile),
                    "Избери JPG, JPEG, PNG или WEBP снимка.");
            }
            if (imageFile.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError(
                    nameof(animal.ImageFile),
                    "Снимката не трябва да е по-голяма от 5 MB.");
            }
        }
        if (ModelState.IsValid)
        {
            if (imageFile is { Length: > 0 } && extension is not null)
            {
                var uploadFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "animals");
                Directory.CreateDirectory(uploadFolder);
                var fileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(uploadFolder, fileName);
                await using var stream = System.IO.File.Create(filePath);
                await imageFile.CopyToAsync(stream);
                animal.ImagePath = $"/uploads/animals/{fileName}";
            }
            _context.Add(animal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        PopulateDropdowns(animal.BreedId, animal.OwnerId);
        return View(animal);
    }
    private void PopulateDropdowns(
        int? selectedBreedId = null,
        int? selectedOwnerId = null)
    {
        ViewData["BreedId"] = new SelectList(
            _context.Breeds,
            "BreedId",
            "Name",
            selectedBreedId);
        var owners = _context.Owners
            .Select(owner => new
            {
                owner.OwnerId,
                FullName = owner.FirstName + " " + owner.LastName
            })
            .ToList();
        ViewData["OwnerId"] = new SelectList(
            owners,
            "OwnerId",
            "FullName",
            selectedOwnerId);
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
