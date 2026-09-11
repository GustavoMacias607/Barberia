using Barberia.Application.DTOs.WorkingHours;
using Barberia.Application.Enums;
using Barberia.Application.Interfaces.Repositories;
using Barberia.Application.Results;
using Barberia.Domain.Entities;

namespace Barberia.Application.Services;

public class WorkingHourService
{
    private readonly IWorkingHourRepository _workingHourRepository;
    private readonly IBarberRepository _barberRepository;

    public WorkingHourService(IWorkingHourRepository workingHourRepository, IBarberRepository barberRepository)
    {
        _workingHourRepository = workingHourRepository;
        _barberRepository = barberRepository;
    }
    public async Task<CreateWorkingHourResult> CreateAsync(CreateWorkingHourRequest request)
    {
        var barber = await _barberRepository.GetByIdAsync(request.BarberId);
        if(barber is null)
        {
            return new(
                CreateWorkingHourStatus.BarberNotFound,
                null);
        }

        if(request.StartTime >= request.EndTime)
        {
            return new(
                CreateWorkingHourStatus.InvalidTime,
                null);
        }

        bool hasConflict = await _workingHourRepository.HasConflictAsync(
            request.BarberId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime);

        if (hasConflict)
        {
            return new(
                CreateWorkingHourStatus.Conflict,
                null);
        }

        WorkingHour workingHour = new(
            request.BarberId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime);

        var created = await _workingHourRepository.CreateAsync(workingHour);
        return new(
                CreateWorkingHourStatus.Success,
                created);
    }
    public async Task<WorkingHour?> GetByIdAsync(int id)
    {
        return await _workingHourRepository.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var workingHour = await _workingHourRepository.GetByIdAsync(id);

        if(workingHour is null)
        {
            return false;
        }

        await _workingHourRepository.DeleteAsync(id);
        return true;
    }

    public async Task<GetWorkingHoursByBarberResult> GetByBarberIdAsync(int barberId)
    {
        var barber = await _barberRepository.GetByIdAsync(barberId);

        if (barber is null)
        {
            return new(
                GetWorkingHoursByBarberStatus.BarberNotFound,
                null);
        }

        var workingHours = await _workingHourRepository.GetByBarberIdAsync(barberId);

        return new(
            GetWorkingHoursByBarberStatus.Success,
            workingHours);
    }
}