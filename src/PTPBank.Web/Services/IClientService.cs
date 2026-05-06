using PTPBank.Web.Models.Common;
using PTPBank.Web.Models.DTOs;
using PTPBank.Web.Models.Requests;

namespace PTPBank.Web.Services;

public interface IClientService
{
    Task<PagedResult<ClientDto>> SearchAsync(string? search, int page, int pageSize, CancellationToken ct = default);
    Task<ClientDto?> GetByIdAsync(int code, CancellationToken ct = default);
    Task<int> CreateAsync(CreateClientRequest request, CancellationToken ct = default);
    Task UpdateAsync(UpdateClientRequest request, CancellationToken ct = default);
    Task DeleteAsync(int code, CancellationToken ct = default);
}
