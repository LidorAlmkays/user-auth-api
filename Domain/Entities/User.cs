namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public string Salt { get; set; }
    public required string Password { get; set; }
}
