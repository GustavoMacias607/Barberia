using Barberia.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Barberia.Infrastructure.Persistence;

public class BarberiaDbContext : DbContext

{
    public BarberiaDbContext(DbContextOptions<BarberiaDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(BarberiaDbContext).Assembly);
    }


    public DbSet<Customer> Customers { get; set; }
    public DbSet<Barber> Barbers { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<WorkingHour> WorkingHours { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

}

