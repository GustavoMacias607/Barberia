using Barberia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barberia.Infrastructure.Persistence.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.Price).HasPrecision(10, 2);
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
               "CK_Service_Price_Positive",
               "Price > 0");
            table.HasCheckConstraint(
                "CK_Service_DurationMinutes_Positive",
                "DurationMinutes > 0");
        });
    }
}

