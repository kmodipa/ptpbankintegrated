using PTPBank.Domain.Entities;
using PTPBank.Domain.Enums;

namespace PTPBank.Domain.Factories;

public class TransactionFactory : ITransactionFactory
{
    public Transaction Create(int accountCode, DateTime transactionDate, decimal amount, string description, TransactionType transactionType)
    {
        return new Transaction
        {
            AccountCode = accountCode,
            TransactionDate = transactionDate,
            CaptureDate = DateTime.UtcNow,
            Amount = amount,
            Description = description.Trim(),
            TransactionType = transactionType,
            CreatedDateUtc = DateTime.UtcNow,
            ModifiedDateUtc = DateTime.UtcNow
        };
    }
}
