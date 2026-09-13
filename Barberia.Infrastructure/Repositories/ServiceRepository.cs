using Barberia.Application.Interfaces.Repositories;
using Barberia.Domain.Entities;
using Barberia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Barberia.Infrastructure.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly BarberiaDbContext _dbContext;

    public ServiceRepository(BarberiaDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<Service>> GetAllAsync()
    {
        return await _dbContext.Services.ToListAsync();
    }
    public async Task<Service?> GetByIdAsync(int id)
    {
        return await _dbContext.Services.FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<Service?> GetByNameAsync(string name)
    {
        return await _dbContext.Services.FirstOrDefaultAsync(x => x.Name == name);
    }
    public async Task<Service> CreateAsync(Service service)
    {
         _dbContext.Services.Add(service);
        await _dbContext.SaveChangesAsync();
        return service;
    }
    public async Task<Service> UpdateAsync(Service service)
    {
        await _dbContext.SaveChangesAsync();
        return service;
    }
}
