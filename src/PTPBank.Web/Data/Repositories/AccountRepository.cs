using PTPBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PTPBank.Web.Data.Repositories;

public class AccountRepository(ApplicationDbContext context) : Repository<Account>(context), IAccountRepository
{
    public async Task<IReadOnlyCollection<Account>> ListByClientAsync(int clientCode, CancellationToken ct = default)
        => await Context.Accounts
            .Where(x => x.ClientCode == clientCode)
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedDateUtc)
            .ToListAsync(ct);

    public async Task<Account?> GetWithTransactionsAsync(int accountCode, CancellationToken ct = default)
        => await Context.Accounts
            .Include(x => x.Client)
            .Include(x => x.Transactions.OrderByDescending(t => t.TransactionDate))
            .FirstOrDefaultAsync(x => x.Code == accountCode, ct);

    public async Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken ct = default)
        => await Context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == accountNumber, ct);
}
