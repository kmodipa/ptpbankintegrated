using PTPBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PTPBank.Web.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        builder.HasKey(x => x.Code);

        builder.Property(x => x.AccountNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.OutstandingBalance).HasColumnType("decimal(18,2)");
        builder.Property(x => x.AccountType).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.AccountStatus).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.CreatedDateUtc).HasColumnType("datetime2");
        builder.Property(x => x.ModifiedDateUtc).HasColumnType("datetime2");
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasOne(x => x.Client)
            .WithMany(x => x.Accounts)
            .HasForeignKey(x => x.ClientCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.AccountNumber).IsUnique();
        builder.HasIndex(x => x.ClientCode);
        builder.HasIndex(x => x.AccountStatus);
    }
}
