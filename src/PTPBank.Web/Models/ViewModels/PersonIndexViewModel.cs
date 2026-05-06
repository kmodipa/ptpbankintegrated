using PTPBank.Web.Models.Common;
using PTPBank.Web.Models.DTOs;

namespace PTPBank.Web.Models.ViewModels;

public class PersonIndexViewModel
{
    public string? Search { get; set; }
    public int CurrentPage { get; set; } = 1;
    public PagedResult<ClientDto> Result { get; set; } = new();
}
