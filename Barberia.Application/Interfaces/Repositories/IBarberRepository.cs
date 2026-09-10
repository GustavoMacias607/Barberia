using Barberia.Domain.Entities;

namespace Barberia.Application.Interfaces.Repositories;

public interface IBarberRepository
{
    Task<IEnumerable<Barber>> GetAllAsync();
    Task<Barber?> GetByIdAsync(int id);
    Task<Barber> CreateAsync(Barber barber);
    Task<Barber> UpdateAsync(Barber barber);
    Task<Barber> ActivateAsync(Barber barber);
    Task<Barber> DeactivateAsync(Barber barber);
}

