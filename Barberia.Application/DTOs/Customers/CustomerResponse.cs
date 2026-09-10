namespace Barberia.Application.DTOs.Customers;

public class CustomerResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public DateTime CreatedAt { get; set; }

    public CustomerResponse(int id, string name, string phone, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Phone = phone;
        CreatedAt = createdAt;
    }
}

