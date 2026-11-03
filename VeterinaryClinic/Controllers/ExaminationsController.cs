
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Models;
using VeterinaryClinic.Data;

[Authorize]
public class ExaminationsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ExaminationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Examinations
    public async Task<IActionResult> Index()
    {
        var examinations = await _context.Examinations
            .Include(e => e.Animal)
            .Include(e => e.Doctor)
            .ToListAsync();

        return View(examinations);
    }

    // GET: Examinations/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var examination = await _context.Examinations
            .Include(e => e.Animal)
            .Include(e => e.Doctor)
            .FirstOrDefaultAsync(e => e.ExaminationId == id);

        if (examination == null)
            return NotFound();

        return View(examination);
    }

    // GET: Examinations/Create
    public IActionResult Create()
    {
        PopulateDropdowns();
        return View();
    }

    // POST: Examinations/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("AnimalId,DoctorId,ExamDate,ExamTime,Description,Diagnosis,Recommendations")]
        Examination examination)
    {
        if (ModelState.IsValid)
        {
            _context.Examinations.Add(examination);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        PopulateDropdowns(examination.AnimalId, examination.DoctorId);
        return View(examination);
    }

    // GET: Examinations/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var examination = await _context.Examinations.FindAsync(id);

        if (examination == null)
            return NotFound();

        PopulateDropdowns(examination.AnimalId, examination.DoctorId);

        return View(examination);
    }

    // POST: Examinations/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("ExaminationId,AnimalId,DoctorId,ExamDate,ExamTime,Description,Diagnosis,Recommendations")]
        Examination input)
    {
        if (id != input.ExaminationId)
            return NotFound();

        var examination = await _context.Examinations.FindAsync(id);

        if (examination == null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            PopulateDropdowns(input.AnimalId, input.DoctorId);
            return View(input);
        }

        examination.AnimalId = input.AnimalId;
        examination.DoctorId = input.DoctorId;
        examination.ExamDate = input.ExamDate;
        examination.ExamTime = input.ExamTime;
        examination.Description = input.Description;
        examination.Diagnosis = input.Diagnosis;
        examination.Recommendations = input.Recommendations;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: Examinations/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var examination = await _context.Examinations
            .Include(e => e.Animal)
            .Include(e => e.Doctor)
            .FirstOrDefaultAsync(e => e.ExaminationId == id);

        if (examination == null)
            return NotFound();

        return View(examination);
    }

    // POST: Examinations/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var examination = await _context.Examinations.FindAsync(id);

        if (examination == null)
            return NotFound();

        _context.Examinations.Remove(examination);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private void PopulateDropdowns(
        int? selectedAnimalId = null,
        string? selectedDoctorId = null)
    {
        var animals = _context.Animals
            .Select(animal => new
            {
                animal.AnimalId,
                DisplayName = animal.Name + " (" + animal.Breed.Name + ")"
            })
            .ToList();

        ViewData["AnimalId"] = new SelectList(
            animals,
            "AnimalId",
            "DisplayName",
            selectedAnimalId);

        var doctors = _context.Users
            .Select(doctor => new
            {
                doctor.Id,
                DisplayName = doctor.FullName
                    ?? doctor.Email
                    ?? doctor.UserName
                    ?? doctor.Id
            })
            .ToList();

        ViewData["DoctorId"] = new SelectList(
            doctors,
            "Id",
            "DisplayName",
            selectedDoctorId);
    }
}
