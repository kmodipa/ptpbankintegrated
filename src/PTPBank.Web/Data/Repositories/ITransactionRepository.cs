using PTPBank.Domain.Entities;

namespace PTPBank.Web.Data.Repositories;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IReadOnlyCollection<Transaction>> ListByAccountAsync(int accountCode, CancellationToken ct = default);
}
