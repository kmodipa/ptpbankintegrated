using PTPBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PTPBank.Web.Data.Repositories;

public class TransactionRepository(ApplicationDbContext context) : Repository<Transaction>(context), ITransactionRepository
{
    public async Task<IReadOnlyCollection<Transaction>> ListByAccountAsync(int accountCode, CancellationToken ct = default)
        => await Context.Transactions
            .Where(x => x.AccountCode == accountCode)
            .AsNoTracking()
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync(ct);
}
