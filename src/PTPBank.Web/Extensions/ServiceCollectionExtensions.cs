using FluentValidation;
using PTPBank.Domain.Factories;
using PTPBank.Web.Data;
using PTPBank.Web.Data.Repositories;
using PTPBank.Web.Data.UnitOfWork;
using PTPBank.Web.Services;
using PTPBank.Web.Validators;
using Microsoft.EntityFrameworkCore;

namespace PTPBank.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPTPBankServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IClientFactory, ClientFactory>();
        services.AddScoped<IAccountFactory, AccountFactory>();
        services.AddScoped<ITransactionFactory, TransactionFactory>();

        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITransactionService, TransactionService>();

        services.AddScoped<IValidator<Models.Requests.CreateClientRequest>, CreateClientRequestValidator>();
        services.AddScoped<IValidator<Models.Requests.UpdateClientRequest>, UpdateClientRequestValidator>();
        services.AddScoped<IValidator<Models.Requests.CreateAccountRequest>, CreateAccountRequestValidator>();
        services.AddScoped<IValidator<Models.Requests.UpdateAccountStatusRequest>, UpdateAccountStatusRequestValidator>();
        services.AddScoped<IValidator<Models.Requests.CreateTransactionRequest>, CreateTransactionRequestValidator>();

        return services;
    }
}
