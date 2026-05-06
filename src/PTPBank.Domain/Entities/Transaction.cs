using PTPBank.Domain.Common;
using PTPBank.Domain.Enums;

namespace PTPBank.Domain.Entities;

public class Transaction : EntityBase
{
    public int AccountCode { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime CaptureDate { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public TransactionType TransactionType { get; set; }

    public Account? Account { get; set; }
}
