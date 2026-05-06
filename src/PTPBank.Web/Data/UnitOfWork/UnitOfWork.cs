using PTPBank.Web.Data.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace PTPBank.Web.Data.UnitOfWork;

public class UnitOfWork(
    ApplicationDbContext context,
    IClientRepository clients,
    IAccountRepository accounts,
    ITransactionRepository transactions) : IUnitOfWork
{
    public IClientRepository Clients { get; } = clients;
    public IAccountRepository Accounts { get; } = accounts;
    public ITransactionRepository Transactions { get; } = transactions;

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is PTPBank.Domain.Common.EntityBase
                        && (e.State == Microsoft.EntityFrameworkCore.EntityState.Added || e.State == Microsoft.EntityFrameworkCore.EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (PTPBank.Domain.Common.EntityBase)entry.Entity;
            entity.ModifiedDateUtc = DateTime.UtcNow;
            if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Added)
            {
                entity.CreatedDateUtc = DateTime.UtcNow;
            }
        }

        return await context.SaveChangesAsync(ct);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
        => context.Database.BeginTransactionAsync(ct);

    public void Dispose() => context.Dispose();
}
