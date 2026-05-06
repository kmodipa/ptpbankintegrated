using PTPBank.Domain.Entities;

namespace PTPBank.Domain.Factories;

public interface IClientFactory
{
    Client Create(string idNumber, string? name, string? surname);
}
