using Barberia.Application.Interfaces.Repositories;
using Barberia.Domain.Entities;
using Barberia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Barberia.Infrastructure.Repositories;

public class BarberRepository : IBarberRepository
{
    private readonly BarberiaDbContext _dbContext;

    public BarberRepository(BarberiaDbContext dbContext)
    {
        _dbContext = dbContext;
    }
     public async Task<IEnumerable<Barber>> GetAllAsync()
    {
        return await _dbContext.Barbers.ToListAsync();
    }
    public async Task<Barber?> GetByIdAsync(int id)
    {
        return await _dbContext.Barbers.FirstOrDefaultAsync(x => x.Id == id);
    } 
    public async Task<Barber> CreateAsync(Barber barber)
    {
        _dbContext.Barbers.Add(barber);
        await _dbContext.SaveChangesAsync();
        return barber;
    }
    public async Task<Barber> UpdateAsync(Barber barber)
    {
        await _dbContext.SaveChangesAsync();
        return barber;
    }
    public async Task<Barber> ActivateAsync(Barber barber)
    {
        await _dbContext.SaveChangesAsync();
        return barber;
    }
    public async Task<Barber> DeactivateAsync(Barber barber)
    {
        await _dbContext.SaveChangesAsync();
        return barber;
    }
}

