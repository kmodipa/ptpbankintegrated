using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PTPBank.Domain.Enums;
using PTPBank.Web.Models.Requests;
using PTPBank.Web.Models.ViewModels;
using PTPBank.Web.Services;

namespace PTPBank.Web.Controllers;

[AutoValidateAntiforgeryToken]
public class TransactionsController(
    ITransactionService transactionService,
    IAccountService accountService,
    ILogger<TransactionsController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int accountId, CancellationToken ct)
    {
        var vm = await BuildIndexViewModel(accountId, ct);
        return vm is null ? NotFound() : View(vm);
    }

    [HttpPost]
    public Task<IActionResult> Deposit(int accountId, TransactionFormViewModel input, CancellationToken ct)
    {
        input.AccountCode = accountId;
        input.TransactionType = TransactionType.Credit;
        return PostTransaction(input, ct);
    }

    [HttpPost]
    public Task<IActionResult> Withdraw(int accountId, TransactionFormViewModel input, CancellationToken ct)
    {
        input.AccountCode = accountId;
        input.TransactionType = TransactionType.Debit;
        return PostTransaction(input, ct);
    }

    private async Task<IActionResult> PostTransaction(TransactionFormViewModel input, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var invalidVm = await BuildIndexViewModel(input.AccountCode, ct, input);
            return invalidVm is null ? NotFound() : View("Index", invalidVm);
        }

        try
        {
            await transactionService.CreateAsync(new CreateTransactionRequest
            {
                AccountCode = input.AccountCode,
                Amount = input.Amount,
                Description = input.Description,
                TransactionDate = input.TransactionDate,
                TransactionType = input.TransactionType
            }, ct);

            TempData["Success"] = "Transaction posted successfully.";
            return RedirectToAction(nameof(Index), new { accountId = input.AccountCode });
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
            logger.LogError(ex, "Transaction post failed for account {AccountId}", input.AccountCode);
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        var vm = await BuildIndexViewModel(input.AccountCode, ct, input);
        return vm is null ? NotFound() : View("Index", vm);
    }

    private async Task<TransactionIndexViewModel?> BuildIndexViewModel(int accountId, CancellationToken ct, TransactionFormViewModel? input = null)
    {
        var account = await accountService.GetByIdAsync(accountId, ct);
        if (account is null)
        {
            return null;
        }

        var transactions = await transactionService.ListByAccountAsync(accountId, ct);

        return new TransactionIndexViewModel
        {
            AccountId = accountId,
            PersonId = account.ClientCode,
            CurrentBalance = account.OutstandingBalance,
            Transactions = transactions,
            Input = input is null
                ? new TransactionFormViewModel { AccountCode = accountId, TransactionDate = DateTime.Today }
                : input
        };
    }
}
