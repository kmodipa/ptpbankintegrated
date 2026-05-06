using FluentValidation;
using PTPBank.Domain.Factories;
using PTPBank.Web.Data;
using PTPBank.Web.Data.Identity;
using PTPBank.Web.Data.Repositories;
using PTPBank.Web.Data.UnitOfWork;
using PTPBank.Web.Services;
using PTPBank.Web.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PTPBank.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPTPBankServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = configuration.GetValue<int?>("Identity:Password:RequiredLength") ?? 6;
            options.Password.RequireDigit = configuration.GetValue<bool?>("Identity:Password:RequireDigit") ?? true;
            options.Password.RequireLowercase = configuration.GetValue<bool?>("Identity:Password:RequireLowercase") ?? true;
            options.Password.RequireUppercase = configuration.GetValue<bool?>("Identity:Password:RequireUppercase") ?? true;
            options.Password.RequireNonAlphanumeric = configuration.GetValue<bool?>("Identity:Password:RequireNonAlphanumeric") ?? false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/Login";
            options.SlidingExpiration = true;
        });

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
