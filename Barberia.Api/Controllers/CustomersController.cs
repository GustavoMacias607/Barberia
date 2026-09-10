using Barberia.Application.DTOs.Customers;
using Barberia.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Barberia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _customerService;

    public CustomersController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IEnumerable<CustomerResponse>> GetAll()
    {
        var result = await _customerService.GetAllAsync();

        return result.Select(customer => new CustomerResponse(
            customer.Id,
            customer.Name,
            customer.Phone,
            customer.CreatedAt
            ));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerResponse>> GetById(int id)
    {
        var result = await _customerService.GetByIdAsync(id);
       
        if(result is null)
        {
            return NotFound();
        }
       
        CustomerResponse response = new(
           result.Id,
           result.Name,
           result.Phone,
           result.CreatedAt
       );
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create(CreateCustomerRequest customer)
    {
        var result = await _customerService.CreateCustomerAsync(customer);

        CustomerResponse response = new (
            result.Id,
            result.Name,
            result.Phone,
            result.CreatedAt
        );

        return CreatedAtAction(nameof(GetById), new {id = response.Id}, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerResponse>> Update(int id, UpdateCustomerRequest customer)
    {
        var result = await _customerService.UpdateCustomerAsync(id, customer);

        if( result is null)
        {
            return NotFound();
        }

        CustomerResponse response = new(
            result.Id,
            result.Name,
            result.Phone,
            result.CreatedAt
        );

        return Ok(response);
    }
}
