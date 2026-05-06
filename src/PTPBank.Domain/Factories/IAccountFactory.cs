using PTPBank.Domain.Entities;
using PTPBank.Domain.Enums;

namespace PTPBank.Domain.Factories;

public interface IAccountFactory
{
    Account Create(int clientCode, string accountNumber, AccountType accountType, decimal openingDeposit);
}
