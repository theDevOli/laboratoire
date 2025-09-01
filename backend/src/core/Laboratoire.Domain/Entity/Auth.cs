namespace Laboratoire.Domain.Entity;

public class Auth
{
    public Guid? UserId { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public byte[]? PasswordHash { get; set; }
}
