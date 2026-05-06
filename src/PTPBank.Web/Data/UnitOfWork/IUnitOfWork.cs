using Microsoft.EntityFrameworkCore.Storage;
using PTPBank.Web.Data.Repositories;

namespace PTPBank.Web.Data.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IClientRepository Clients { get; }
    IAccountRepository Accounts { get; }
    ITransactionRepository Transactions { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
}
