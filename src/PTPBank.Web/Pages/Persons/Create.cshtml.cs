using FluentValidation;
using PTPBank.Web.Models.Requests;
using PTPBank.Web.Models.ViewModels;
using PTPBank.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PTPBank.Web.Pages.Persons;

public class CreateModel(IClientService service, ILogger<CreateModel> logger) : PageModel
{
    [BindProperty]
    public ClientFormViewModel Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            await service.CreateAsync(new CreateClientRequest
            {
                Name = Input.Name,
                Surname = Input.Surname,
                IDNumber = Input.IDNumber
            }, ct);

            TempData["Success"] = "Person created successfully.";
            return RedirectToPage("Index");
        }
        catch (ValidationException ex)
        {
            foreach (var error in ex.Errors)
            {
                ModelState.AddModelError(string.Empty, error.ErrorMessage);
            }
            return Page();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Create person failed");
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
