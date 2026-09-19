namespace CartAPI.Features.Accounts.Auth.Application.Abstractions;

public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string passwordHash, string password);
}
