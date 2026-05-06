using PTPBank.Domain.Entities;
using PTPBank.Domain.Enums;

namespace PTPBank.Domain.Factories;

public interface ITransactionFactory
{
    Transaction Create(int accountCode, DateTime transactionDate, decimal amount, string description, TransactionType transactionType);
}
