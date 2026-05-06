using PTPBank.Domain.Entities;
using PTPBank.Domain.Enums;

namespace PTPBank.Domain.Factories;

public class AccountFactory : IAccountFactory
{
    public Account Create(int clientCode, string accountNumber, AccountType accountType, decimal openingDeposit)
    {
        return new Account
        {
            ClientCode = clientCode,
            AccountNumber = accountNumber.Trim(),
            AccountType = accountType,
            AccountStatus = AccountStatus.Open,
            OutstandingBalance = openingDeposit,
            CreatedDateUtc = DateTime.UtcNow,
            ModifiedDateUtc = DateTime.UtcNow
        };
    }
}
