using FluentValidation;
using PTPBank.Domain.Enums;
using PTPBank.Web.Data.Repositories;
using PTPBank.Web.Models.Requests;

namespace PTPBank.Web.Validators;

public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountRequestValidator(IAccountRepository accountRepo)
    {
        RuleFor(x => x.ClientCode).GreaterThan(0);

        RuleFor(x => x.AccountNumber)
            .NotEmpty()
            .MaximumLength(50)
            .MustAsync(async (number, ct) => await accountRepo.GetByAccountNumberAsync(number, ct) is null)
            .WithMessage("Account number already exists.");

        RuleFor(x => x.OpeningDeposit)
            .GreaterThan(0)
            .Must((request, amount) =>
                request.AccountType == AccountType.Savings ? amount >= 200m : amount >= 500m)
            .WithMessage("Minimum opening deposit is R200 for Savings and R500 for Cheque.");
    }
}
