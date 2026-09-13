using Barberia.Application.DTOs.Services;
using Barberia.Application.Enums;
using Barberia.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Barberia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceController : ControllerBase
{

    private readonly ServiceService _serviceService;

    public ServiceController(ServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpGet]
    public async Task<IEnumerable<ServiceResponse>> GetAll()
    {
        var result = await _serviceService.GetAllAsync();

        return result.Select(x => new ServiceResponse(
            x.Id,
            x.Name,
            x.Price,
            x.DurationMinutes,
            x.IsActive,
            x.CreatedAt,
            x.Description
            ));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse>> GetById(int id)
    {
        var result = await _serviceService.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        ServiceResponse response = new(
            result.Id,
            result.Name,
            result.Price,
            result.DurationMinutes,
            result.IsActive,
            result.CreatedAt,
            result.Description);

        return Ok(response);
    }
    [HttpPost]
    public async Task<ActionResult<ServiceResponse>> Create(CreateServiceRequest request)
    {
        var result = await _serviceService.CreateAsync(request);
        if (result.status == CreateServiceStatus.NameAlreadyExists)
        {
            return Conflict();
        }

        ServiceResponse response = new(
            result.service!.Id,
            result.service.Name,
            result.service.Price,
            result.service.DurationMinutes,
            result.service.IsActive,
            result.service.CreatedAt,
            result.service.Description);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ServiceResponse>> Update(int id, UpdateServiceRequest request)
    {
        var result = await _serviceService.UpdateAsync(id, request);

        if (result.status == UpdateServiceStatus.NotFound)
        {
            return NotFound();
        }

        if (result.status == UpdateServiceStatus.NameAlreadyExists)
        {
            return Conflict();
        }

        ServiceResponse response = new(
            result.service!.Id,
            result.service.Name,
            result.service.Price,
            result.service.DurationMinutes,
            result.service.IsActive,
            result.service.CreatedAt,
            result.service.Description);

        return Ok(response);
    }

    [HttpPatch("{id}/activate")]
    public async Task<ActionResult> Activate(int id)
    {
        var result = await _serviceService.ActivateAsync(id);
        if(result is null)
        {
            return NotFound();
        }

        ServiceResponse response = new(
            result.Id,
            result.Name,
            result.Price,
            result.DurationMinutes,
            result.IsActive,
            result.CreatedAt,
            result.Description);

        return Ok(response);
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<ActionResult> Deactivate(int id)
    {
        var result = await _serviceService.DeactivateAsync(id);
        if (result is null)
        {
            return NotFound();
        }

        ServiceResponse response = new(
            result.Id,
            result.Name,
            result.Price,
            result.DurationMinutes,
            result.IsActive,
            result.CreatedAt,
            result.Description);

        return Ok(response);
    }
}
