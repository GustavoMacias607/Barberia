using Barberia.Application.DTOs.Barber;
using Barberia.Application.Interfaces.Repositories;
using Barberia.Domain.Entities;

namespace Barberia.Application.Services;

public class BarberService
{
    private readonly IBarberRepository _barberRepository;

    public BarberService(IBarberRepository barberRepository)
    {
        _barberRepository = barberRepository;
    }

    public async Task<IEnumerable<Barber>> GetAllAsync()
    {
        return await _barberRepository.GetAllAsync();
    }

    public async Task<Barber?> GetByIdAsync(int id)
    {
        return await _barberRepository.GetByIdAsync(id);
    }

    public async Task<Barber> CreateAsync(CreateBarberRequest request)
    {
        Barber barber = new(request.Name);
        return await _barberRepository.CreateAsync(barber);
    }

    public async Task<Barber?> UpdateAsync(int id, UpdateBarberRequest request)
    {
        var barber = await _barberRepository.GetByIdAsync(id);
        
        if(barber is null)
        {
            return null;
        }

        barber.Update(request.Name);
        return await _barberRepository.UpdateAsync(barber);
    }

    public async Task<Barber?> ActivateAsync(int id)
    {
        var barber = await _barberRepository.GetByIdAsync(id);

        if (barber is null)
        {
            return null;
        }

        if (barber.IsActive)
        {
            return barber;
        }

        barber.Activate();
        return await _barberRepository.ActivateAsync(barber);
    }

    public async Task<Barber?> DeactivateAsync(int id)
    {
        var barber = await _barberRepository.GetByIdAsync(id);

        if (barber is null)
        {
            return null;
        }

        if (!barber.IsActive)
        {
            return barber;
        }

        barber.Deactivate();
        return await _barberRepository.DeactivateAsync(barber);
    }
}
