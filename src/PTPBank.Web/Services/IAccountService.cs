using PTPBank.Web.Models.DTOs;
using PTPBank.Web.Models.Requests;

namespace PTPBank.Web.Services;

public interface IAccountService
{
    Task<IReadOnlyCollection<AccountDto>> ListByClientAsync(int clientCode, CancellationToken ct = default);
    Task<AccountDto?> GetByIdAsync(int accountCode, CancellationToken ct = default);
    Task<int> CreateAsync(CreateAccountRequest request, CancellationToken ct = default);
    Task UpdateStatusAsync(UpdateAccountStatusRequest request, CancellationToken ct = default);
    Task<decimal> GetBalanceAsync(int accountCode, CancellationToken ct = default);
}
