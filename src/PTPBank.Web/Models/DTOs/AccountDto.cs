using PTPBank.Domain.Enums;

namespace PTPBank.Web.Models.DTOs;

public class AccountDto
{
    public int Code { get; set; }
    public int ClientCode { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public AccountStatus AccountStatus { get; set; }
    public decimal OutstandingBalance { get; set; }
}
