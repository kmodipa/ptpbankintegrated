using PTPBank.Domain.Enums;

namespace PTPBank.Web.Models.DTOs;

public class TransactionDto
{
    public int Code { get; set; }
    public int AccountCode { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime CaptureDate { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public TransactionType TransactionType { get; set; }
}
