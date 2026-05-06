using FluentValidation;
using PTPBank.Web.Data.Repositories;
using PTPBank.Web.Models.Requests;

namespace PTPBank.Web.Validators;

public class CreateClientRequestValidator : AbstractValidator<CreateClientRequest>
{
    public CreateClientRequestValidator(IClientRepository repo)
    {
        RuleFor(x => x.IDNumber)
            .NotEmpty()
            .MaximumLength(50)
            .MustAsync(async (id, ct) => await repo.GetByIdNumberAsync(id, ct) is null)
            .WithMessage("A client with this ID number already exists.");

        RuleFor(x => x.Name).MaximumLength(50);
        RuleFor(x => x.Surname).MaximumLength(50);
    }
}
