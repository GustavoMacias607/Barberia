using Barberia.Domain.Entities;

namespace Barberia.Application.Interfaces.Repositories;

public interface IServiceRepository
{
    Task<IEnumerable<Service>> GetAllAsync();
    Task<Service?> GetByIdAsync(int id);
    Task<Service?> GetByNameAsync(string name);
    Task<Service> CreateAsync(Service service);
    Task<Service> UpdateAsync(Service service);
}
