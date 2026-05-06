using PTPBank.Domain.Common;

namespace PTPBank.Domain.Entities;

public class Client : EntityBase
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string IDNumber { get; set; } = string.Empty;

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}
