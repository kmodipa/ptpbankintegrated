using FluentValidation;
using PTPBank.Domain.Enums;
using PTPBank.Domain.Factories;
using PTPBank.Web.Data.UnitOfWork;
using PTPBank.Web.Models.DTOs;
using PTPBank.Web.Models.Requests;

namespace PTPBank.Web.Services;

public class TransactionService(
    IUnitOfWork uow,
    ITransactionFactory transactionFactory,
    IValidator<CreateTransactionRequest> validator) : ITransactionService
{
    public async Task<IReadOnlyCollection<TransactionDto>> ListByAccountAsync(int accountCode, CancellationToken ct = default)
    {
        var list = await uow.Transactions.ListByAccountAsync(accountCode, ct);
        return list.Select(x => new TransactionDto
        {
            Code = x.Code,
            AccountCode = x.AccountCode,
            TransactionDate = x.TransactionDate,
            CaptureDate = x.CaptureDate,
            Amount = x.Amount,
            Description = x.Description,
            TransactionType = x.TransactionType
        }).ToList();
    }

    public async Task<int> CreateAsync(CreateTransactionRequest request, CancellationToken ct = default)
    {
        await validator.ValidateAndThrowAsync(request, ct);
        var account = await uow.Accounts.GetByIdAsync(request.AccountCode, ct)
            ?? throw new InvalidOperationException("Account not found.");

        if (account.AccountStatus == AccountStatus.Closed)
        {
            throw new InvalidOperationException("No transactions may be posted to closed accounts.");
        }

        var transaction = transactionFactory.Create(
            request.AccountCode,
            request.TransactionDate,
            request.Amount,
            request.Description,
            request.TransactionType);

        await using var trx = await uow.BeginTransactionAsync(ct);
        await uow.Transactions.AddAsync(transaction, ct);

        // Credit increases balance; debit decreases balance.
        var signedAmount = request.TransactionType == TransactionType.Credit
            ? Math.Abs(request.Amount)
            : -Math.Abs(request.Amount);

        var nextBalance = account.OutstandingBalance + signedAmount;
        if (nextBalance < 0)
        {
            throw new InvalidOperationException("Transaction would result in negative account balance.");
        }

        account.OutstandingBalance = nextBalance;
        uow.Accounts.Update(account);

        await uow.SaveChangesAsync(ct);
        await trx.CommitAsync(ct);

        return transaction.Code;
    }
}
