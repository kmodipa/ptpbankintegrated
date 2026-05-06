using System.ComponentModel.DataAnnotations;
using PTPBank.Domain.Enums;

namespace PTPBank.Web.Models.ViewModels;

public class TransactionFormViewModel
{
    public int AccountCode { get; set; }

    [Required, DataType(DataType.Date), Display(Name = "Transaction Date")]
    public DateTime TransactionDate { get; set; } = DateTime.Today;

    [Range(typeof(decimal), "-999999999", "999999999"), Display(Name = "Amount")]
    public decimal Amount { get; set; }

    [Required, MaxLength(100)]
    public string Description { get; set; } = string.Empty;

    [Required, Display(Name = "Transaction Type")]
    public TransactionType TransactionType { get; set; } = TransactionType.Credit;
}
