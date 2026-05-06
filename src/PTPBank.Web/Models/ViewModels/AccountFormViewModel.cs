using System.ComponentModel.DataAnnotations;
using PTPBank.Domain.Enums;

namespace PTPBank.Web.Models.ViewModels;

public class AccountFormViewModel
{
    public int ClientCode { get; set; }

    [Required, MaxLength(50), Display(Name = "Account Number")]
    public string AccountNumber { get; set; } = string.Empty;

    [Required, Display(Name = "Account Type")]
    public AccountType AccountType { get; set; }

    [Range(0.01, double.MaxValue), Display(Name = "Opening Deposit")]
    public decimal OpeningDeposit { get; set; }
}
