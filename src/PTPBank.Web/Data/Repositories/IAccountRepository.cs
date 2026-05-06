using PTPBank.Domain.Entities;

namespace PTPBank.Web.Data.Repositories;

public interface IAccountRepository : IRepository<Account>
{
    Task<IReadOnlyCollection<Account>> ListByClientAsync(int clientCode, CancellationToken ct = default);
    Task<Account?> GetWithTransactionsAsync(int accountCode, CancellationToken ct = default);
    Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken ct = default);
}
