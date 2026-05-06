using PTPBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PTPBank.Web.Data.Specifications;

public static class ClientSearchSpecification
{
    public static IQueryable<Client> Apply(IQueryable<Client> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var term = search.Trim();
        return query.Where(p =>
            p.IDNumber.Contains(term) ||
            (p.Surname != null && p.Surname.Contains(term)) ||
            p.Accounts.Any(a => a.AccountNumber.Contains(term))
        );
    }
}
