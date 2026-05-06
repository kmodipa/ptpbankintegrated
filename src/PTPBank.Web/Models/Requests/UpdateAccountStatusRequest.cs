using PTPBank.Domain.Enums;

namespace PTPBank.Web.Models.Requests;

public class UpdateAccountStatusRequest
{
    public int AccountCode { get; set; }
    public AccountStatus AccountStatus { get; set; }
}
