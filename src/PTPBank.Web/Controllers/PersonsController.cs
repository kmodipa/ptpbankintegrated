using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PTPBank.Web.Models.Requests;
using PTPBank.Web.Models.ViewModels;
using PTPBank.Web.Services;

namespace PTPBank.Web.Controllers;

[AutoValidateAntiforgeryToken]
public class PersonsController(IClientService personService, ILogger<PersonsController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? search, int currentPage = 1, CancellationToken ct = default)
    {
        var safePage = currentPage <= 0 ? 1 : currentPage;
        var result = await personService.SearchAsync(search, safePage, 10, ct);

        var vm = new PersonIndexViewModel
        {
            Search = search,
            CurrentPage = safePage,
            Result = result
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult Create() => View(new ClientFormViewModel());

    [HttpPost]
    public async Task<IActionResult> Create(ClientFormViewModel input, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(input);
        }

        try
        {
            await personService.CreateAsync(new CreateClientRequest
            {
                Name = input.Name,
                Surname = input.Surname,
                IDNumber = input.IDNumber
            }, ct);

            TempData["Success"] = "Person created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            foreach (var error in ex.Errors)
            {
                ModelState.AddModelError(string.Empty, error.ErrorMessage);
            }

            return View(input);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Create person failed");
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(input);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var person = await personService.GetByIdAsync(id, ct);
        if (person is null)
        {
            return NotFound();
        }

        var vm = new ClientFormViewModel
        {
            Code = person.Code,
            Name = person.Name,
            Surname = person.Surname,
            IDNumber = person.IDNumber
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, ClientFormViewModel input, CancellationToken ct)
    {
        input.Code = id;

        if (!ModelState.IsValid || input.Code is null)
        {
            return View(input);
        }

        try
        {
            await personService.UpdateAsync(new UpdateClientRequest
            {
                Code = input.Code.Value,
                Name = input.Name,
                Surname = input.Surname,
                IDNumber = input.IDNumber
            }, ct);

            TempData["Success"] = "Person updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            foreach (var error in ex.Errors)
            {
                ModelState.AddModelError(string.Empty, error.ErrorMessage);
            }

            return View(input);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update person failed");
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(input);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            await personService.DeleteAsync(id, ct);
            return Json(new { ok = true });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Delete person failed for id {PersonId}", id);
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return Content(ex.Message);
        }
    }
}
