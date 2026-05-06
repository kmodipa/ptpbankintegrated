using PTPBank.Domain.Enums;

namespace PTPBank.Web.Models.Requests;

public class CreateAccountRequest
{
    public int ClientCode { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public decimal OpeningDeposit { get; set; }
}
