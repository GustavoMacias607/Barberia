using Barberia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Barberia.Infrastructure.Persistence.Configurations;

public class WorkingHourConfiguration : IEntityTypeConfiguration<WorkingHour>
{
    public void Configure(EntityTypeBuilder<WorkingHour> builder)
    {
        builder.Property(x => x.DayOfWeek).HasConversion<byte>();
        builder.HasIndex(x => new { x.BarberId, x.DayOfWeek });
        builder.HasOne<Barber>()
            .WithMany()
            .HasForeignKey(x => x.BarberId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
               "CK_WorkingHour_StartTime_EndTime",
               "StartTime < EndTime");
        });
    }
}

