namespace Barberia.Domain.Entities;

public class Customer
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    public string Phone { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public Customer(string name, string phone)
    {
        Name = name;
        Phone = phone;
        CreatedAt = DateTime.Now;
    }
}