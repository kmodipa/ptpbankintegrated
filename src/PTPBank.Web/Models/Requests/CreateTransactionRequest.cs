using PTPBank.Domain.Enums;

namespace PTPBank.Web.Models.Requests;

public class CreateTransactionRequest
{
    public int AccountCode { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public TransactionType TransactionType { get; set; }
}
