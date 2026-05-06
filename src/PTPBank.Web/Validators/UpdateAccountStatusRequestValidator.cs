using FluentValidation;
using PTPBank.Web.Models.Requests;

namespace PTPBank.Web.Validators;

public class UpdateAccountStatusRequestValidator : AbstractValidator<UpdateAccountStatusRequest>
{
    public UpdateAccountStatusRequestValidator()
    {
        RuleFor(x => x.AccountCode).GreaterThan(0);
    }
}
