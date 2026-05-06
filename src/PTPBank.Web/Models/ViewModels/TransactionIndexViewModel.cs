using PTPBank.Web.Models.DTOs;

namespace PTPBank.Web.Models.ViewModels;

public class TransactionIndexViewModel
{
    public int AccountId { get; set; }
    public int PersonId { get; set; }
    public decimal CurrentBalance { get; set; }
    public IReadOnlyCollection<TransactionDto> Transactions { get; set; } = Array.Empty<TransactionDto>();
    public TransactionFormViewModel Input { get; set; } = new();
}
