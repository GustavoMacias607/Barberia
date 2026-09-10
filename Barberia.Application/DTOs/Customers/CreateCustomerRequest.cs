using System.ComponentModel.DataAnnotations;

namespace Barberia.Application.DTOs.Customers;

public class CreateCustomerRequest
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }
    [Required]
    [RegularExpression("^[0-9]{10}$")]
    public required string Phone { get; set; }
}

