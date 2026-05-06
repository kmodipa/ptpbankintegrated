using PTPBank.Domain.Entities;
using PTPBank.Web.Data.Specifications;
using Microsoft.EntityFrameworkCore;

namespace PTPBank.Web.Data.Repositories;

public class ClientRepository(ApplicationDbContext context) : Repository<Client>(context), IClientRepository
{
    public async Task<Client?> GetWithAccountsAsync(int code, CancellationToken ct = default)
        => await Context.Clients
            .Include(x => x.Accounts)
            .ThenInclude(x => x.Transactions)
            .FirstOrDefaultAsync(x => x.Code == code, ct);

    public async Task<Client?> GetByIdNumberAsync(string idNumber, CancellationToken ct = default)
        => await Context.Clients.FirstOrDefaultAsync(x => x.IDNumber == idNumber, ct);

    public async Task<(IReadOnlyCollection<Client> Items, int Total)> SearchPagedAsync(string? search, int page, int pageSize, CancellationToken ct = default)
    {
        var query = Context.Clients.Include(p => p.Accounts).AsQueryable();
        query = ClientSearchSpecification.Apply(query, search);

        var total = await query.CountAsync(ct);
        var items = await query
            .AsNoTracking()
            .OrderBy(p => p.Surname)
            .ThenBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }
}
