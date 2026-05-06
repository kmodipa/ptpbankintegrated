using System.Linq.Expressions;
using PTPBank.Domain.Common;

namespace PTPBank.Web.Data.Repositories;

public interface IRepository<T> where T : EntityBase
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyCollection<T>> ListAsync(CancellationToken ct = default);
    Task<IReadOnlyCollection<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);
}
