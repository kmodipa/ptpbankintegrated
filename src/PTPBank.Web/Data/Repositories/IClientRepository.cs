using PTPBank.Domain.Entities;

namespace PTPBank.Web.Data.Repositories;

public interface IClientRepository : IRepository<Client>
{
    Task<Client?> GetWithAccountsAsync(int code, CancellationToken ct = default);
    Task<Client?> GetByIdNumberAsync(string idNumber, CancellationToken ct = default);
    Task<(IReadOnlyCollection<Client> Items, int Total)> SearchPagedAsync(string? search, int page, int pageSize, CancellationToken ct = default);
}
