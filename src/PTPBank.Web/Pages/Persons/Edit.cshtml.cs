using FluentValidation;
using PTPBank.Web.Models.Requests;
using PTPBank.Web.Models.ViewModels;
using PTPBank.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PTPBank.Web.Pages.Persons;

public class EditModel(IClientService service, ILogger<EditModel> logger) : PageModel
{
    [BindProperty]
    public ClientFormViewModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int code, CancellationToken ct)
    {
        var person = await service.GetByIdAsync(code, ct);
        if (person is null) return NotFound();

        Input = new ClientFormViewModel
        {
            Code = person.Code,
            Name = person.Name,
            Surname = person.Surname,
            IDNumber = person.IDNumber
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid || Input.Code is null) return Page();

        try
        {
            await service.UpdateAsync(new UpdateClientRequest
            {
                Code = Input.Code.Value,
                Name = Input.Name,
                Surname = Input.Surname,
                IDNumber = Input.IDNumber
            }, ct);

            TempData["Success"] = "Person updated successfully.";
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
            logger.LogError(ex, "Update person failed");
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
