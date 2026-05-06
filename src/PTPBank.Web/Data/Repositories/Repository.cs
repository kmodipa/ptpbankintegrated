using System.Linq.Expressions;
using PTPBank.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace PTPBank.Web.Data.Repositories;

public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : EntityBase
{
    protected readonly ApplicationDbContext Context = context;

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await Context.Set<T>().FirstOrDefaultAsync(x => x.Code == id, ct);

    public async Task<IReadOnlyCollection<T>> ListAsync(CancellationToken ct = default)
        => await Context.Set<T>().AsNoTracking().ToListAsync(ct);

    public async Task<IReadOnlyCollection<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await Context.Set<T>().Where(predicate).AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await Context.Set<T>().AddAsync(entity, ct);

    public void Update(T entity) => Context.Set<T>().Update(entity);

    public void Remove(T entity) => Context.Set<T>().Remove(entity);
}
