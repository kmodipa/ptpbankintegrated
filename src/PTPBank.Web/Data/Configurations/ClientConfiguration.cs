using PTPBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PTPBank.Web.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");
        builder.HasKey(x => x.Code);

        builder.Property(x => x.Name).HasMaxLength(50);
        builder.Property(x => x.Surname).HasMaxLength(50);
        builder.Property(x => x.IDNumber).HasColumnName("IDNumber").HasMaxLength(50).IsRequired();
        builder.Property(x => x.CreatedDateUtc).HasColumnType("datetime2");
        builder.Property(x => x.ModifiedDateUtc).HasColumnType("datetime2");
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasIndex(x => x.IDNumber).IsUnique();
        builder.HasIndex(x => x.Surname);
    }
}
