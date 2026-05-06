using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PTPBank.Domain.Enums;
using PTPBank.Web.Models.Requests;
using PTPBank.Web.Models.ViewModels;
using PTPBank.Web.Services;

namespace PTPBank.Web.Controllers;

[AutoValidateAntiforgeryToken]
public class AccountsController(IAccountService accountService, ILogger<AccountsController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int personId, CancellationToken ct)
    {
        var vm = new AccountIndexViewModel
        {
            PersonId = personId,
            Accounts = await accountService.ListByClientAsync(personId, ct)
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult Create(int personId)
    {
        return View(new AccountFormViewModel { ClientCode = personId });
    }

    [HttpPost]
    public async Task<IActionResult> Create(AccountFormViewModel input, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(input);
        }

        try
        {
            await accountService.CreateAsync(new CreateAccountRequest
            {
                ClientCode = input.ClientCode,
                AccountNumber = input.AccountNumber,
                AccountType = input.AccountType,
                OpeningDeposit = input.OpeningDeposit
            }, ct);

            TempData["Success"] = "Account opened successfully.";
            return RedirectToAction(nameof(Index), new { personId = input.ClientCode });
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
            logger.LogError(ex, "Open account failed");
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(input);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Close(int accountId, CancellationToken ct)
    {
        try
        {
            await accountService.UpdateStatusAsync(new UpdateAccountStatusRequest
            {
                AccountCode = accountId,
                AccountStatus = AccountStatus.Closed
            }, ct);

            return Json(new { ok = true });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Close account failed for account {AccountId}", accountId);
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return Content(ex.Message);
        }
    }
}
