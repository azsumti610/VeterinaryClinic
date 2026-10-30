
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Models;
using VeterinaryClinic.Data;

public class AnimalTypesController : Controller
{
    private readonly ApplicationDbContext _context;

    public AnimalTypesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: ANIMALTYPES
    public async Task<IActionResult> Index()    
    {
        return View(await _context.AnimalTypes.ToListAsync());
    }

    // GET: ANIMALTYPES/Details/5
    public async Task<IActionResult> Details(int? animaltypeid)
    {
        if (animaltypeid == null)
        {
            return NotFound();
        }

        var animaltype = await _context.AnimalTypes
            .FirstOrDefaultAsync(m => m.AnimalTypeId == animaltypeid);
        if (animaltype == null)
        {
            return NotFound();
        }

        return View(animaltype);
    }

    // GET: ANIMALTYPES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: ANIMALTYPES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AnimalTypeId,Name,Description,Breeds")] AnimalType animaltype)
    {
        if (ModelState.IsValid)
        {
            _context.Add(animaltype);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(animaltype);
    }

    // GET: ANIMALTYPES/Edit/5
    public async Task<IActionResult> Edit(int? animaltypeid)
    {
        if (animaltypeid == null)
        {
            return NotFound();
        }

        var animaltype = await _context.AnimalTypes.FindAsync(animaltypeid);
        if (animaltype == null)
        {
            return NotFound();
        }
        return View(animaltype);
    }

    // POST: ANIMALTYPES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? animaltypeid, [Bind("AnimalTypeId,Name,Description,Breeds")] AnimalType animaltype)
    {
        if (animaltypeid != animaltype.AnimalTypeId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(animaltype);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalTypeExists(animaltype.AnimalTypeId))
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
        return View(animaltype);
    }

    // GET: ANIMALTYPES/Delete/5
    public async Task<IActionResult> Delete(int? animaltypeid)
    {
        if (animaltypeid == null)
        {
            return NotFound();
        }

        var animaltype = await _context.AnimalTypes
            .FirstOrDefaultAsync(m => m.AnimalTypeId == animaltypeid);
        if (animaltype == null)
        {
            return NotFound();
        }

        return View(animaltype);
    }

    // POST: ANIMALTYPES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? animaltypeid)
    {
        var animaltype = await _context.AnimalTypes.FindAsync(animaltypeid);
        if (animaltype != null)
        {
            _context.AnimalTypes.Remove(animaltype);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AnimalTypeExists(int? animaltypeid)
    {
        return _context.AnimalTypes.Any(e => e.AnimalTypeId == animaltypeid);
    }
}
