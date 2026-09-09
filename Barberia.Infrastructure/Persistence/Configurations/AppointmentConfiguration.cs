using Barberia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barberia.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.Property(x => x.Status)
            .HasConversion<byte>();

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Barber>()
            .WithMany()
            .HasForeignKey(x => x.BarberId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Service>()
            .WithMany()
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.BarberId, x.StartAt });

        builder.HasIndex(x => new { x.CustomerId, x.StartAt });

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_Appointment_DurationMinutes_Positive",
                "DurationMinutes > 0");
        });

    }
}