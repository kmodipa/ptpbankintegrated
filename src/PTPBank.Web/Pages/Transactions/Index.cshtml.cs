using FluentValidation;
using PTPBank.Web.Data.Repositories;
using PTPBank.Web.Models.DTOs;
using PTPBank.Web.Models.Requests;
using PTPBank.Web.Models.ViewModels;
using PTPBank.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PTPBank.Web.Pages.Transactions;

public class IndexModel(
    ITransactionService transactionService,
    IAccountService accountService,
    IAccountRepository accountRepository,
    ILogger<IndexModel> logger) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int AccountCode { get; set; }

    public int ClientCode { get; set; }
    public decimal CurrentBalance { get; set; }

    public IReadOnlyCollection<TransactionDto> Transactions { get; set; } = Array.Empty<TransactionDto>();

    [BindProperty]
    public TransactionFormViewModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        await LoadPageData(ct);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await LoadPageData(ct);
            return Page();
        }

        try
        {
            await transactionService.CreateAsync(new CreateTransactionRequest
            {
                AccountCode = Input.AccountCode,
                Amount = Input.Amount,
                Description = Input.Description,
                TransactionDate = Input.TransactionDate,
                TransactionType = Input.TransactionType
            }, ct);

            TempData["Success"] = "Transaction posted successfully.";
            return RedirectToPage(new { accountCode = Input.AccountCode });
        }
        catch (ValidationException ex)
        {
            foreach (var error in ex.Errors)
            {
                ModelState.AddModelError(string.Empty, error.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Transaction post failed");
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        await LoadPageData(ct);
        return Page();
    }

    public async Task<IActionResult> OnGetBalanceAsync(CancellationToken ct)
    {
        var balance = await accountService.GetBalanceAsync(AccountCode, ct);
        return new JsonResult(new { balance });
    }

    private async Task LoadPageData(CancellationToken ct)
    {
        var account = await accountRepository.GetByIdAsync(AccountCode, ct);
        if (account is null)
        {
            throw new InvalidOperationException("Account not found");
        }

        ClientCode = account.ClientCode;
        CurrentBalance = account.OutstandingBalance;
        Transactions = await transactionService.ListByAccountAsync(AccountCode, ct);
        Input = new TransactionFormViewModel { AccountCode = AccountCode, TransactionDate = DateTime.Today };
    }
}
