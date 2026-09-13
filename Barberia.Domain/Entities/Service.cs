
namespace Barberia.Domain.Entities;

public class Service
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int DurationMinutes { get; private set; }

    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Service(string name,decimal price, int durationMinutes, string? description = null)
    {
        Name = name;
        Price = price;
        Description = description;
        DurationMinutes = durationMinutes;
        IsActive = true;
        CreatedAt = DateTime.Now;
    }

    public void Update(string name, decimal price, int durationMinutes, string? description = null)
    {
        Name = name;
        Price = price;
        Description = description;
        DurationMinutes = durationMinutes;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
