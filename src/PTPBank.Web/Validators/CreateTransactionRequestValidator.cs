using FluentValidation;
using PTPBank.Web.Models.Requests;

namespace PTPBank.Web.Validators;

public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
{
    public CreateTransactionRequestValidator()
    {
        RuleFor(x => x.AccountCode).GreaterThan(0);
        RuleFor(x => x.TransactionDate).LessThanOrEqualTo(_ => DateTime.Today);
        RuleFor(x => x.Amount).NotEqual(0m);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(100);
    }
}
