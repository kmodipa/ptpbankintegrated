using PTPBank.Web.Models.DTOs;
using PTPBank.Web.Models.Requests;

namespace PTPBank.Web.Services;

public interface ITransactionService
{
    Task<IReadOnlyCollection<TransactionDto>> ListByAccountAsync(int accountCode, CancellationToken ct = default);
    Task<int> CreateAsync(CreateTransactionRequest request, CancellationToken ct = default);
}
