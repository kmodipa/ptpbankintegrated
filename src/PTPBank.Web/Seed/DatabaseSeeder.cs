using PTPBank.Domain.Enums;
using PTPBank.Web.Data;
using PTPBank.Web.Models.Requests;
using PTPBank.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace PTPBank.Web.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Database migration failed during startup seeding.");
        }

        if (!configuration.GetValue<bool>("SeedData:Enabled"))
        {
            return;
        }

        try
        {
            var clientService = scope.ServiceProvider.GetRequiredService<IClientService>();
            var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
            var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();

            var existing = await clientService.SearchAsync("8601015009081", 1, 1);
            if (existing.TotalCount > 0)
            {
                return;
            }

            var clientCode = await clientService.CreateAsync(new CreateClientRequest
            {
                Name = "Sample",
                Surname = "User",
                IDNumber = "8601015009081"
            });

            var accountCode = await accountService.CreateAsync(new CreateAccountRequest
            {
                ClientCode = clientCode,
                AccountNumber = "10009999",
                AccountType = AccountType.Savings,
                OpeningDeposit = 500m
            });

            await transactionService.CreateAsync(new CreateTransactionRequest
            {
                AccountCode = accountCode,
                TransactionDate = DateTime.Today,
                Amount = 100m,
                Description = "Sample debit",
                TransactionType = TransactionType.Debit
            });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Sample data seeding skipped because database is not ready or already constrained.");
        }
    }
}
