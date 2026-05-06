using PTPBank.Web.Models.DTOs;

namespace PTPBank.Web.Models.ViewModels;

public class AccountIndexViewModel
{
    public int PersonId { get; set; }
    public IReadOnlyCollection<AccountDto> Accounts { get; set; } = Array.Empty<AccountDto>();
}
