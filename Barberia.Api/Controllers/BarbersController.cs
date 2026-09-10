using Barberia.Application.DTOs.Barber;
using Barberia.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Barberia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BarbersController : ControllerBase
{
    private readonly BarberService _barberService;

    public BarbersController(BarberService barberService)
    {
        _barberService = barberService;
    }

    [HttpGet]
    public async Task<IEnumerable<BarberResponse>> GetAll()
    {
        var result =  await _barberService.GetAllAsync();

        return result.Select(x => new BarberResponse(
            x.Id,
            x.Name,
            x.IsActive,
            x.CreatedAt
            ));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BarberResponse>> GetById(int id)
    {
        var result = await _barberService.GetByIdAsync(id);
        if(result is null)
        {
            return NotFound();
        }

        BarberResponse barber = new(
            result.Id,
            result.Name,
            result.IsActive,
            result.CreatedAt
            );

        return Ok(barber);
    }
    [HttpPost]
    public async Task<ActionResult<BarberResponse>> Create(CreateBarberRequest barber)
    {
        var result = await _barberService.CreateAsync(barber);

        BarberResponse response = new(
           result.Id,
           result.Name,
           result.IsActive,
           result.CreatedAt
           );

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BarberResponse>> Update(int id,UpdateBarberRequest barber)
    {
        var result = await _barberService.UpdateAsync(id, barber);

        if( result is null)
        {
            return NotFound();
        }

        BarberResponse response = new(
          result.Id,
          result.Name,
          result.IsActive,
          result.CreatedAt
          );

        return Ok(response);
    }

    [HttpPatch("{id}/activate")]
    public async Task<ActionResult<BarberResponse>> Activate(int id)
    {
        var result = await _barberService.ActivateAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        BarberResponse response = new(
          result.Id,
          result.Name,
          result.IsActive,
          result.CreatedAt
          );

        return Ok(response);
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<ActionResult<BarberResponse>> Deactivate(int id)
    {
        var result = await _barberService.DeactivateAsync(id);

        if (result is null)
        {
            return NotFound();
        }

        BarberResponse response = new(
          result.Id,
          result.Name,
          result.IsActive,
          result.CreatedAt
          );

        return Ok(response);
    }
}

