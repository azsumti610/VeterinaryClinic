using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Data;
using VeterinaryClinic.Models;

[Authorize(Roles = AppRoles.Doctor + "," + AppRoles.User)]
public class AnimalsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<AnimalsController> _logger;

    public AnimalsController(
        ApplicationDbContext context,
        IWebHostEnvironment environment,
        ILogger<AnimalsController> logger)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
    }

    private bool IsDoctor => User.IsInRole(AppRoles.Doctor);

    // Само собствениците, до които акаунтът има достъп.
    private IQueryable<Owner> AccessibleOwners()
    {
        if (IsDoctor)
        {
            return _context.Owners;
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return _context.Owners.Where(owner => false);
        }

        return _context.Owners.Where(
            owner => owner.ApplicationUserId == userId);
    }

    // Проверката се използва и за директните адреси,
    // не само за списъка с животни.
    private IQueryable<Animal> AccessibleAnimals()
    {
        var animals = _context.Animals
            .Include(animal => animal.Breed)
            .Include(animal => animal.Owner);

        if (IsDoctor)
        {
            return animals;
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return animals.Where(animal => false);
        }

        return animals.Where(animal =>
            animal.Owner != null &&
            animal.Owner.ApplicationUserId == userId);
    }

    private async Task<int?> CurrentOwnerIdAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        return await _context.Owners
            .Where(owner => owner.ApplicationUserId == userId)
            .Select(owner => (int?)owner.OwnerId)
            .SingleOrDefaultAsync();
    }

    public async Task<IActionResult> Index()
    {
        if (!IsDoctor && await CurrentOwnerIdAsync() == null)
        {
            return RedirectToAction("Index", "Profile");
        }

        var animals = await AccessibleAnimals()
            .AsNoTracking()
            .ToListAsync();

        return View(animals);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var animal = await AccessibleAnimals()
            .AsNoTracking()
            .SingleOrDefaultAsync(animal => animal.AnimalId == id);

        if (animal == null)
        {
            return NotFound();
        }

        return View(animal);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var animal = new Animal();

        if (!IsDoctor)
        {
            var ownerId = await CurrentOwnerIdAsync();

            if (ownerId == null)
            {
                return RedirectToAction("Index", "Profile");
            }

            animal.OwnerId = ownerId.Value;
        }

        await PopulateDropdownsAsync(animal.BreedId, animal.OwnerId);

        return View(animal);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Name,BreedId,OwnerId,BirthDate,Description,ImageFile")]
        Animal animal)
    {
        if (!IsDoctor)
        {
            var ownerId = await CurrentOwnerIdAsync();

            if (ownerId == null)
            {
                return RedirectToAction("Index", "Profile");
            }

            // Не се доверяваме на OwnerId от формата.
            animal.OwnerId = ownerId.Value;
            ModelState.Remove(nameof(Animal.OwnerId));
        }

        await ValidateAnimalAsync(animal);

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(
                animal.BreedId, animal.OwnerId);

            return View(animal);
        }

        animal.ImagePath = await SavePhotoAsync(animal.ImageFile);

        _context.Animals.Add(animal);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var animal = await AccessibleAnimals()
            .SingleOrDefaultAsync(animal => animal.AnimalId == id);

        if (animal == null)
        {
            return NotFound();
        }

        await PopulateDropdownsAsync(
            animal.BreedId, animal.OwnerId);

        return View(animal);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int? id,
        [Bind("AnimalId,Name,BreedId,OwnerId,BirthDate,Description,ImageFile")]
        Animal input,
        bool removeImage = false)
    {
        if (id == null || id != input.AnimalId)
        {
            return NotFound();
        }

        var animal = await AccessibleAnimals()
            .SingleOrDefaultAsync(animal => animal.AnimalId == id);

        if (animal == null)
        {
            return NotFound();
        }

        if (!IsDoctor)
        {
            // Потребителят не може да прехвърля животното.
            input.OwnerId = animal.OwnerId;
            ModelState.Remove(nameof(Animal.OwnerId));
        }

        await ValidateAnimalAsync(input);

        if (!ModelState.IsValid)
        {
            input.ImagePath = animal.ImagePath;

            await PopulateDropdownsAsync(
                input.BreedId, input.OwnerId);

            return View(input);
        }

        var newImagePath = await SavePhotoAsync(input.ImageFile);

        // Променяме само разрешените полета.
        animal.Name = input.Name;
        animal.BreedId = input.BreedId;
        animal.OwnerId = input.OwnerId;
        animal.BirthDate = input.BirthDate;
        animal.Description = input.Description;

        // Без нов файл запазваме досегашната снимка.
        if (newImagePath != null)
        {
            animal.ImagePath = newImagePath;
        }
        else if (removeImage)
        {
            animal.ImagePath = null;
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var animal = await AccessibleAnimals()
            .AsNoTracking()
            .SingleOrDefaultAsync(animal => animal.AnimalId == id);

        if (animal == null)
        {
            return NotFound();
        }

        return View(animal);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var animal = await AccessibleAnimals()
            .SingleOrDefaultAsync(animal => animal.AnimalId == id);

        if (animal == null)
        {
            return NotFound();
        }

        var hasExaminations = await _context.Examinations
            .AnyAsync(examination =>
                examination.AnimalId == animal.AnimalId);

        if (hasExaminations)
        {
            TempData["AnimalError"] =
                "Животно със записани прегледи не може да бъде изтрито.";

            return RedirectToAction(nameof(Index));
        }

        _context.Animals.Remove(animal);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
        {
            _logger.LogWarning(
                exception,
                "Animal deletion failed.");

            TempData["AnimalError"] =
                "Изтриването не успя. Възможно е междувременно " +
                "да е добавен свързан запис. Презаредете списъка.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync(
        int? selectedBreedId = null,
        int? selectedOwnerId = null)
    {
        var breeds = await _context.Breeds
            .AsNoTracking()
            .OrderBy(breed => breed.Name)
            .ToListAsync();

        ViewData["BreedId"] = new SelectList(
            breeds, "BreedId", "Name", selectedBreedId);

        var owners = await AccessibleOwners()
            .AsNoTracking()
            .Select(owner => new
            {
                owner.OwnerId,
                FullName = owner.FirstName + " " + owner.LastName
            })
            .OrderBy(owner => owner.FullName)
            .ToListAsync();

        ViewData["OwnerId"] = new SelectList(
            owners, "OwnerId", "FullName", selectedOwnerId);
    }

    private async Task ValidateAnimalAsync(Animal animal)
    {
        if (!await _context.Breeds.AnyAsync(
            breed => breed.BreedId == animal.BreedId))
        {
            ModelState.AddModelError(
                nameof(Animal.BreedId),
                "Изберете съществуваща порода.");
        }

        if (!await AccessibleOwners().AnyAsync(
            owner => owner.OwnerId == animal.OwnerId))
        {
            ModelState.AddModelError(
                nameof(Animal.OwnerId),
                "Изберете валиден собственик.");
        }

        var imageFile = animal.ImageFile;

        if (imageFile is not { Length: > 0 })
        {
            return;
        }

        var extension = Path.GetExtension(
            imageFile.FileName).ToLowerInvariant();

        string[] allowedExtensions =
            { ".jpg", ".jpeg", ".png", ".webp" };

        if (!allowedExtensions.Contains(extension))
        {
            ModelState.AddModelError(
                nameof(Animal.ImageFile),
                "Изберете JPG, JPEG, PNG или WEBP снимка.");
        }

        if (imageFile.Length > 5 * 1024 * 1024)
        {
            ModelState.AddModelError(
                nameof(Animal.ImageFile),
                "Снимката не трябва да е по-голяма от 5 MB.");
        }
    }

    private async Task<string?> SavePhotoAsync(IFormFile? imageFile)
    {
        if (imageFile is not { Length: > 0 })
        {
            return null;
        }

        var extension = Path.GetExtension(
            imageFile.FileName).ToLowerInvariant();

        var uploadFolder = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            "animals");

        Directory.CreateDirectory(uploadFolder);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadFolder, fileName);

        await using var stream = System.IO.File.Create(filePath);
        await imageFile.CopyToAsync(stream);

        return $"/uploads/animals/{fileName}";
    }
}