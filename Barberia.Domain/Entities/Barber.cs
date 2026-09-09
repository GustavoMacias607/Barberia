namespace Barberia.Domain.Entities;

public class Barber
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public Barber(string name)
    {
        Name = name;
        IsActive = true;
        CreatedAt = DateTime.Now;
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
