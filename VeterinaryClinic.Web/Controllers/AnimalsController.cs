using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using VeterinaryClinic.Application.DTOs;
using VeterinaryClinic.Application.Interfaces;
using VeterinaryClinic.Web.ViewModels.Animals;

namespace VeterinaryClinic.Web.Controllers;

[Authorize(Roles = "Owner")]
[ResponseCache(
    NoStore = true,
    Location = ResponseCacheLocation.None)]
public sealed class AnimalsController : Controller
{
    private readonly IAnimalService _animals;
    private readonly ILogger<AnimalsController> _logger;

    public AnimalsController(
        IAnimalService animals,
        ILogger<AnimalsController> logger)
    {
        _animals = animals;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? search,
        CancellationToken cancellationToken)
    {
        var ownerId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(ownerId))
        {
            return Challenge();
        }

        if (search?.Length > 100)
        {
            return BadRequest(
                "Търсенето може да съдържа до 100 символа.");
        }

        search = search?.Trim();

        var animals = await _animals.GetMyAnimalsAsync(
            ownerId,
            search,
            cancellationToken);

        return View(new AnimalListViewModel
        {
            Search = search,
            Animals = animals
        });
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateAnimalDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateAnimalDto model,
        CancellationToken cancellationToken)
    {
        var ownerId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(ownerId))
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var animal = await _animals.AddAnimalAsync(
                model,
                ownerId,
                cancellationToken);

            TempData["Success"] =
                "Животното е добавено успешно.";

            return RedirectToAction(
                nameof(Details),
                new { id = animal.Id });
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(
                string.Empty,
                exception.Message);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(
                string.Empty,
                exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogWarning(
                exception,
                "Animal creation was rejected.");

            ModelState.AddModelError(
                string.Empty,
                "Животното не беше записано. " +
                "Провери дали профилът ти е активен собственик.");
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(
        Guid id,
        CancellationToken cancellationToken)
    {
        var ownerId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(ownerId))
        {
            return Challenge();
        }

        var animal = await _animals.FindAnimalAsync(
            id,
            ownerId,
            cancellationToken);

        if (animal is null)
        {
            return NotFound();
        }

        return View(animal);
    }
}