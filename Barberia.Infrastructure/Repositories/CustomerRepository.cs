using Barberia.Application.Interfaces.Repositories;
using Barberia.Domain.Entities;
using Barberia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Barberia.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly BarberiaDbContext _dbContext;

    public CustomerRepository(BarberiaDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _dbContext.Customers
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
       return await _dbContext.Customers.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();

        return customer;
    }

    public  async Task<Customer> UpdateAsync(Customer customer)
    {
        await _dbContext.SaveChangesAsync();
        return customer;
    }
}

