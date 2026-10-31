using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Models;
using VeterinaryClinic.Data;

public class ExaminationsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ExaminationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: EXAMINATIONS
    public async Task<IActionResult> Index()    
    {

        var examinations = await _context.Examinations
        .Include(examination => examination.Animal)
        .Include(examination => examination.Doctor)
        .ToListAsync();
        return View(examinations);
    }

    // GET: EXAMINATIONS/Details/5
    public async Task<IActionResult> Details(int? examinationid)
    {
        if (examinationid == null)
        {
            return NotFound();
        }

        var examination = await _context.Examinations
            .FirstOrDefaultAsync(m => m.ExaminationId == examinationid);
        if (examination == null)
        {
            return NotFound();
        }

        return View(examination);
    }

    // GET: EXAMINATIONS/Create
    public IActionResult Create()
    {
        PopulateDropdowns();
        return View();
    }

    // POST: EXAMINATIONS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ExaminationId,AnimalId,Animal,DoctorId,Doctor,ExamDate,ExamTime,Description,Diagnosis,Recommendations")] Examination examination)
    {
        if (ModelState.IsValid)
        {
            _context.Add(examination);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        PopulateDropdowns(examination.AnimalId, examination.DoctorId);
        return View(examination);

    }
    private void PopulateDropdowns(int? selectedAnimalId = null, string? selectedDoctorId = null)
    {
        var animals = _context.Animals
            .Select(animal => new
            {
                animal.AnimalId,
                DisplayName = animal.Name + " (" + animal.Breed.Name + ")"
            })
            .ToList();
        ViewData["AnimalId"] = new SelectList(
            animals, "AnimalId", "DisplayName", selectedAnimalId);
        var doctors = _context.Users
            .Select(doctor => new
            {
                doctor.Id,
                DisplayName = doctor.FullName ?? doctor.Email ?? doctor.UserName ?? doctor.Id
            })
            .ToList();
        ViewData["DoctorId"] = new SelectList(
            doctors, "Id", "DisplayName", selectedDoctorId);
    }

    // GET: EXAMINATIONS/Edit/5
    public async Task<IActionResult> Edit(int? examinationid)
    {
        if (examinationid == null)
        {
            return NotFound();
        }

        var examination = await _context.Examinations.FindAsync(examinationid);
        if (examination == null)
        {
            return NotFound();
        }
        return View(examination);
    }

    // POST: EXAMINATIONS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? examinationid, [Bind("ExaminationId,AnimalId,Animal,DoctorId,Doctor,ExamDate,ExamTime,Description,Diagnosis,Recommendations")] Examination examination)
    {
        if (examinationid != examination.ExaminationId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(examination);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExaminationExists(examination.ExaminationId))
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
        return View(examination);
    }

    // GET: EXAMINATIONS/Delete/5
    public async Task<IActionResult> Delete(int? examinationid)
    {
        if (examinationid == null)
        {
            return NotFound();
        }

        var examination = await _context.Examinations
            .FirstOrDefaultAsync(m => m.ExaminationId == examinationid);
        if (examination == null)
        {
            return NotFound();
        }

        return View(examination);
    }

    // POST: EXAMINATIONS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? examinationid)
    {
        var examination = await _context.Examinations.FindAsync(examinationid);
        if (examination != null)
        {
            _context.Examinations.Remove(examination);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ExaminationExists(int? examinationid)
    {
        return _context.Examinations.Any(e => e.ExaminationId == examinationid);
    }
}
