using FluentValidation;
using PTPBank.Web.Data.Repositories;
using PTPBank.Web.Models.Requests;

namespace PTPBank.Web.Validators;

public class UpdateClientRequestValidator : AbstractValidator<UpdateClientRequest>
{
    public UpdateClientRequestValidator(IClientRepository repo)
    {
        RuleFor(x => x.Code).GreaterThan(0);

        RuleFor(x => x.IDNumber)
            .NotEmpty()
            .MaximumLength(50)
            .MustAsync(async (request, id, ct) =>
            {
                var existing = await repo.GetByIdNumberAsync(id, ct);
                return existing is null || existing.Code == request.Code;
            })
            .WithMessage("A client with this ID number already exists.");

        RuleFor(x => x.Name).MaximumLength(50);
        RuleFor(x => x.Surname).MaximumLength(50);
    }
}
