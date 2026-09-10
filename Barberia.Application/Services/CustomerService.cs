using Barberia.Application.DTOs.Customers;
using Barberia.Application.Interfaces.Repositories;
using Barberia.Domain.Entities;

namespace Barberia.Application.Services;

public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _customerRepository.GetAllAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _customerRepository.GetByIdAsync(id);
    }

    public async Task<Customer> CreateCustomerAsync(CreateCustomerRequest request)
    {
        Customer customer = new(
            request.Name,
            request.Phone
         );

        return await _customerRepository.CreateAsync(customer);
    }

    public async Task<Customer?> UpdateCustomerAsync(int id, UpdateCustomerRequest request)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer is null)
        {
            return null;
        }

        customer.Update(request.Name, request.Phone);

        return await _customerRepository.UpdateAsync(customer);
    }
}

