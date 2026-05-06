using PTPBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PTPBank.Web.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(x => x.Code);

        builder.Property(x => x.TransactionDate).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.CaptureDate).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.Description).HasMaxLength(100).IsRequired();
        builder.Property(x => x.TransactionType).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.CreatedDateUtc).HasColumnType("datetime2");
        builder.Property(x => x.ModifiedDateUtc).HasColumnType("datetime2");
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasOne(x => x.Account)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.AccountCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.AccountCode);
        builder.HasIndex(x => x.TransactionDate);
    }
}
