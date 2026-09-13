using Barberia.Application.DTOs.Services;
using Barberia.Application.Enums;
using Barberia.Application.Interfaces.Repositories;
using Barberia.Application.Results;
using Barberia.Domain.Entities;

namespace Barberia.Application.Services;

public class ServiceService
{
    private readonly IServiceRepository _serviceRepository;

    public ServiceService(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<IEnumerable<Service>> GetAllAsync()
    {
       return await _serviceRepository.GetAllAsync();
    }

    public async Task<Service?> GetByIdAsync(int id)
    {
        return await _serviceRepository.GetByIdAsync(id);
    }

    public async Task<CreateServiceResult> CreateAsync(CreateServiceRequest request)
    {
        string normalizedName = request.Name.Trim();

        var existingName = await _serviceRepository.GetByNameAsync(normalizedName);
        if(existingName is not null)
        {
            return new CreateServiceResult(CreateServiceStatus.NameAlreadyExists, null);
        }

        Service service = new(normalizedName, request.Price, request.DurationMinutes, request.Description);

        var created = await _serviceRepository.CreateAsync(service);

        return new CreateServiceResult(CreateServiceStatus.Success, created);
    }

    public async Task<UpdateServiceResult> UpdateAsync(int id, UpdateServiceRequest request)
    {
        var service = await _serviceRepository.GetByIdAsync(id);
        if(service is null)
        {
            return new UpdateServiceResult(UpdateServiceStatus.NotFound, null);
        }
        string normalizedName = request.Name.Trim();
        var existingName = await _serviceRepository.GetByNameAsync(normalizedName);
        if (existingName is not null && existingName.Id != service.Id)
        {
            return new UpdateServiceResult(UpdateServiceStatus.NameAlreadyExists, null);
        }
        service.Update(normalizedName, request.Price, request.DurationMinutes, request.Description);

        await _serviceRepository.UpdateAsync(service);

        return new UpdateServiceResult(UpdateServiceStatus.Success, service);
    }

    public async Task<Service?> ActivateAsync(int id)
    {
        var service = await _serviceRepository.GetByIdAsync(id);
        if( service is null )
        {
            return null;
        }

        if (service.IsActive)
        {
            return service;
        }

        service.Activate();
        await _serviceRepository.UpdateAsync(service);
        return service;
    }

    public async Task<Service?> DeactivateAsync(int id)
    {
        var service = await _serviceRepository.GetByIdAsync(id);
        if (service is null)
        {
            return null;
        }

        if (!service.IsActive)
        {
            return service;
        }

        service.Deactivate();
        await _serviceRepository.UpdateAsync(service);
        return service;
    }
}
