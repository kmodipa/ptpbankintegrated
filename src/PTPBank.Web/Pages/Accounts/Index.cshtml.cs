using PTPBank.Domain.Enums;
using PTPBank.Web.Models.DTOs;
using PTPBank.Web.Models.Requests;
using PTPBank.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PTPBank.Web.Pages.Accounts;

public class IndexModel(IAccountService accountService) : PageModel
{
    public IReadOnlyCollection<AccountDto> Accounts { get; private set; } = Array.Empty<AccountDto>();

    [BindProperty(SupportsGet = true)]
    public int ClientCode { get; set; }

    public async Task OnGetAsync(CancellationToken ct)
    {
        Accounts = await accountService.ListByClientAsync(ClientCode, ct);
    }

    public async Task<IActionResult> OnPostStatusAsync(int accountCode, string accountStatus, CancellationToken ct)
    {
        var parsed = Enum.Parse<AccountStatus>(accountStatus, true);
        await accountService.UpdateStatusAsync(new UpdateAccountStatusRequest
        {
            AccountCode = accountCode,
            AccountStatus = parsed
        }, ct);

        return new JsonResult(new { ok = true });
    }
}
