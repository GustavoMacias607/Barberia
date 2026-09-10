namespace Barberia.Application.DTOs.Barber;

public class BarberResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public BarberResponse(int id, string name , bool isActive, DateTime createAt)
    {
        Id = id;
        Name = name;
        IsActive = isActive;
        CreatedAt = createAt;
    }
}

