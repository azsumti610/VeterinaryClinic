
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Models;
using VeterinaryClinic.Data;

public class OwnersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OwnersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: OWNERS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Owners.ToListAsync());
    }

    // GET: OWNERS/Details/5
    public async Task<IActionResult> Details(int? ownerid)
    {
        if (ownerid == null)
        {
            return NotFound();
        }

        var owner = await _context.Owners
            .FirstOrDefaultAsync(m => m.OwnerId == ownerid);
        if (owner == null)
        {
            return NotFound();
        }

        return View(owner);
    }

    // GET: OWNERS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: OWNERS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("OwnerId,FirstName,LastName,PhoneNumber,Email,Address,Animals")] Owner owner)
    {
        if (ModelState.IsValid)
        {
            _context.Add(owner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(owner);
    }

    // GET: OWNERS/Edit/5
    public async Task<IActionResult> Edit(int? ownerid)
    {
        if (ownerid == null)
        {
            return NotFound();
        }

        var owner = await _context.Owners.FindAsync(ownerid);
        if (owner == null)
        {
            return NotFound();
        }
        return View(owner);
    }

    // POST: OWNERS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? ownerid, [Bind("OwnerId,FirstName,LastName,PhoneNumber,Email,Address,")] Owner owner)
    {
        if (ownerid != owner.OwnerId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(owner);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OwnerExists(owner.OwnerId))
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
        return View(owner);
    }

    // GET: OWNERS/Delete/5
    public async Task<IActionResult> Delete(int? ownerid)
    {
        if (ownerid == null)
        {
            return NotFound();
        }

        var owner = await _context.Owners
            .FirstOrDefaultAsync(m => m.OwnerId == ownerid);
        if (owner == null)
        {
            return NotFound();
        }

        return View(owner);
    }

    // POST: OWNERS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? ownerid)
    {
        var owner = await _context.Owners.FindAsync(ownerid);
        if (owner != null)
        {
            _context.Owners.Remove(owner);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool OwnerExists(int? ownerid)
    {
        return _context.Owners.Any(e => e.OwnerId == ownerid);
    }
}
