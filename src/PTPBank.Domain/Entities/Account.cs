using PTPBank.Domain.Common;
using PTPBank.Domain.Enums;

namespace PTPBank.Domain.Entities;

public class Account : EntityBase
{
    public int ClientCode { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public AccountStatus AccountStatus { get; set; } = AccountStatus.Open;
    public decimal OutstandingBalance { get; set; }

    public Client? Client { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public bool CanClose() => OutstandingBalance == 0m;
}
