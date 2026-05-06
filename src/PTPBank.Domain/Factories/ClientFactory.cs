using PTPBank.Domain.Entities;

namespace PTPBank.Domain.Factories;

public class ClientFactory : IClientFactory
{
    public Client Create(string idNumber, string? name, string? surname)
    {
        return new Client
        {
            IDNumber = idNumber.Trim(),
            Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim(),
            Surname = string.IsNullOrWhiteSpace(surname) ? null : surname.Trim(),
            CreatedDateUtc = DateTime.UtcNow,
            ModifiedDateUtc = DateTime.UtcNow
        };
    }
}
