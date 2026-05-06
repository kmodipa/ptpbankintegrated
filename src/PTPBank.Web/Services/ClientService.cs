using FluentValidation;
using PTPBank.Domain.Factories;
using PTPBank.Web.Data.UnitOfWork;
using PTPBank.Web.Models.Common;
using PTPBank.Web.Models.DTOs;
using PTPBank.Web.Models.Requests;
using Microsoft.Extensions.Logging;

namespace PTPBank.Web.Services;

public class ClientService(
    IUnitOfWork uow,
    IClientFactory clientFactory,
    IValidator<CreateClientRequest> createValidator,
    IValidator<UpdateClientRequest> updateValidator,
    ILogger<ClientService> logger) : IClientService
{
    public async Task<PagedResult<ClientDto>> SearchAsync(string? search, int page, int pageSize, CancellationToken ct = default)
    {
        var (items, total) = await uow.Clients.SearchPagedAsync(search, page, pageSize, ct);
        return new PagedResult<ClientDto>
        {
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = items.Select(x => new ClientDto
            {
                Code = x.Code,
                Name = x.Name,
                Surname = x.Surname,
                IDNumber = x.IDNumber,
                AccountCount = x.Accounts.Count
            }).ToList()
        };
    }

    public async Task<ClientDto?> GetByIdAsync(int code, CancellationToken ct = default)
    {
        var client = await uow.Clients.GetByIdAsync(code, ct);
        return client is null ? null : new ClientDto
        {
            Code = client.Code,
            Name = client.Name,
            Surname = client.Surname,
            IDNumber = client.IDNumber
        };
    }

    public async Task<int> CreateAsync(CreateClientRequest request, CancellationToken ct = default)
    {
        await createValidator.ValidateAndThrowAsync(request, ct);
        var client = clientFactory.Create(request.IDNumber, request.Name, request.Surname);
        await uow.Clients.AddAsync(client, ct);
        await uow.SaveChangesAsync(ct);
        logger.LogInformation("Created client {ClientCode}", client.Code);
        return client.Code;
    }

    public async Task UpdateAsync(UpdateClientRequest request, CancellationToken ct = default)
    {
        await updateValidator.ValidateAndThrowAsync(request, ct);
        var client = await uow.Clients.GetByIdAsync(request.Code, ct) ?? throw new InvalidOperationException("Client not found.");
        client.Name = request.Name?.Trim();
        client.Surname = request.Surname?.Trim();
        client.IDNumber = request.IDNumber.Trim();
        uow.Clients.Update(client);
        await uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int code, CancellationToken ct = default)
    {
        var client = await uow.Clients.GetWithAccountsAsync(code, ct) ?? throw new InvalidOperationException("Client not found.");
        if (client.Accounts.Any(a => a.AccountStatus == PTPBank.Domain.Enums.AccountStatus.Open))
        {
            throw new InvalidOperationException("Only clients with no accounts or all closed accounts may be deleted.");
        }

        uow.Clients.Remove(client);
        await uow.SaveChangesAsync(ct);
    }
}
