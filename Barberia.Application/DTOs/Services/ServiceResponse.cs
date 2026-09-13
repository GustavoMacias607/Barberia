namespace Barberia.Application.DTOs.Services;

public class ServiceResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public ServiceResponse(
        int id,
        string name,
        decimal price,
        int durationMinutes,
        bool isActive,
        DateTime createdAt,
        string? description = null)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        DurationMinutes = durationMinutes;
        IsActive = isActive;
        CreatedAt = createdAt;
    }
}
