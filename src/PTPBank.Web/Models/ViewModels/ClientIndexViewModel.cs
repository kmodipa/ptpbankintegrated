using PTPBank.Web.Models.Common;
using PTPBank.Web.Models.DTOs;

namespace PTPBank.Web.Models.ViewModels;

public class ClientIndexViewModel
{
    public string? Search { get; set; }
    public PagedResult<ClientDto> Result { get; set; } = new();
}
