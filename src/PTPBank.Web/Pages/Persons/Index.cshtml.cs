using PTPBank.Web.Models.Common;
using PTPBank.Web.Models.DTOs;
using PTPBank.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PTPBank.Web.Pages.Persons;

public class IndexModel(IClientService personService) : PageModel
{
    public PagedResult<ClientDto> Result { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public async Task OnGetAsync(CancellationToken ct)
    {
        Result = await personService.SearchAsync(Search, CurrentPage <= 0 ? 1 : CurrentPage, 10, ct);
    }

    public async Task<IActionResult> OnPostDeleteAsync(int code, CancellationToken ct)
    {
        await personService.DeleteAsync(code, ct);
        return new JsonResult(new { ok = true });
    }
}
