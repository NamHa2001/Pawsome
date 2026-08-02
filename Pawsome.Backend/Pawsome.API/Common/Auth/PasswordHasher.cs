namespace Pawsome.API.Common.Auth;

public interface IPasswordHasher
{
    string HashPassword(string plainPassword);
    bool VerifyPassword(string plainPassword, string passwordHash);
}

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string plainPassword) =>
        BCrypt.Net.BCrypt.HashPassword(plainPassword);

    public bool VerifyPassword(string plainPassword, string passwordHash) =>
        BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);
}
