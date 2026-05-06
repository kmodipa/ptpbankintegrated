using FluentValidation;
using PTPBank.Domain.Enums;
using PTPBank.Domain.Factories;
using PTPBank.Web.Data.UnitOfWork;
using PTPBank.Web.Models.DTOs;
using PTPBank.Web.Models.Requests;

namespace PTPBank.Web.Services;

public class AccountService(
    IUnitOfWork uow,
    IAccountFactory factory,
    ITransactionFactory transactionFactory,
    IValidator<CreateAccountRequest> createValidator,
    IValidator<UpdateAccountStatusRequest> statusValidator) : IAccountService
{
    public async Task<IReadOnlyCollection<AccountDto>> ListByClientAsync(int clientCode, CancellationToken ct = default)
    {
        var list = await uow.Accounts.ListByClientAsync(clientCode, ct);
        return list.Select(Map).ToList();
    }

    public async Task<AccountDto?> GetByIdAsync(int accountCode, CancellationToken ct = default)
    {
        var account = await uow.Accounts.GetByIdAsync(accountCode, ct);
        return account is null ? null : Map(account);
    }

    public async Task<int> CreateAsync(CreateAccountRequest request, CancellationToken ct = default)
    {
        await createValidator.ValidateAndThrowAsync(request, ct);

        var client = await uow.Clients.GetByIdAsync(request.ClientCode, ct);
        if (client is null)
        {
            throw new InvalidOperationException("Client does not exist.");
        }

        await using var trx = await uow.BeginTransactionAsync(ct);

        var account = factory.Create(request.ClientCode, request.AccountNumber, request.AccountType, request.OpeningDeposit);
        await uow.Accounts.AddAsync(account, ct);
        await uow.SaveChangesAsync(ct);

        var openingTransaction = transactionFactory.Create(
            account.Code,
            DateTime.Today,
            request.OpeningDeposit,
            "Opening deposit",
            TransactionType.Credit);

        await uow.Transactions.AddAsync(openingTransaction, ct);
        await uow.SaveChangesAsync(ct);

        await trx.CommitAsync(ct);
        return account.Code;
    }

    public async Task UpdateStatusAsync(UpdateAccountStatusRequest request, CancellationToken ct = default)
    {
        await statusValidator.ValidateAndThrowAsync(request, ct);

        var account = await uow.Accounts.GetByIdAsync(request.AccountCode, ct)
            ?? throw new InvalidOperationException("Account not found.");

        if (request.AccountStatus == AccountStatus.Closed && account.OutstandingBalance != 0m)
        {
            throw new InvalidOperationException("Cannot close account with non-zero balance.");
        }

        if (request.AccountStatus == AccountStatus.Open && account.AccountStatus == AccountStatus.Open)
        {
            throw new InvalidOperationException("Account is already open.");
        }

        account.AccountStatus = request.AccountStatus;
        uow.Accounts.Update(account);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<decimal> GetBalanceAsync(int accountCode, CancellationToken ct = default)
    {
        var account = await uow.Accounts.GetByIdAsync(accountCode, ct)
            ?? throw new InvalidOperationException("Account not found.");
        return account.OutstandingBalance;
    }

    private static AccountDto Map(PTPBank.Domain.Entities.Account account) => new()
    {
        Code = account.Code,
        ClientCode = account.ClientCode,
        AccountNumber = account.AccountNumber,
        AccountType = account.AccountType,
        AccountStatus = account.AccountStatus,
        OutstandingBalance = account.OutstandingBalance
    };
}
