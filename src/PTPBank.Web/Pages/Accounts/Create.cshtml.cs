using FluentValidation;
using PTPBank.Web.Models.Requests;
using PTPBank.Web.Models.ViewModels;
using PTPBank.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PTPBank.Web.Pages.Accounts;

public class CreateModel(IAccountService accountService, ILogger<CreateModel> logger) : PageModel
{
    [BindProperty]
    public AccountFormViewModel Input { get; set; } = new();

    public void OnGet(int clientCode)
    {
        Input.ClientCode = clientCode;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            await accountService.CreateAsync(new CreateAccountRequest
            {
                ClientCode = Input.ClientCode,
                AccountNumber = Input.AccountNumber,
                AccountType = Input.AccountType,
                OpeningDeposit = Input.OpeningDeposit
            }, ct);

            TempData["Success"] = "Account opened successfully.";
            return RedirectToPage("Index", new { clientCode = Input.ClientCode });
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
            logger.LogError(ex, "Open account failed");
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}
