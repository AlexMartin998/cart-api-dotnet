using CartAPI.Features.Accounts.Auth.Application.Abstractions;
using CartAPI.Features.Accounts.Auth.Application.Commands;
using CartAPI.Features.Accounts.Auth.Domain;
using CartAPI.Features.Accounts.Auth.Domain.ValueObjects;
using CartAPI.Shared.Domain.Errors;

namespace CartAPI.Tests.Unit.Accounts.Auth;

public sealed class LoginCommandHandlerTests
{
    private static readonly User Ana = User.Register(
        Email.Create("ana@cartapi.local"), "Ana", Roles.Customer, "hash:secret123", DateTime.UtcNow);

    private readonly LoginCommandHandler _handler = new(new FakeUsers(Ana), new FakeHasher(), new FakeTokens());

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsABearerToken()
    {
        var result = await _handler.HandleAsync(new LoginCommand(" ANA@cartapi.local ", "secret123"), CancellationToken.None);

        Assert.Equal("Bearer", result.TokenType);
        Assert.Equal("token-for-ana@cartapi.local", result.AccessToken);
    }

    [Theory]
    [InlineData("ana@cartapi.local", "wrong")]
    [InlineData("nobody@cartapi.local", "secret123")]
    public async Task Login_UnknownEmailAndWrongPassword_FailExactlyTheSameWay(string email, string password)
    {
        var error = await Assert.ThrowsAsync<UnauthorizedException>(
            () => _handler.HandleAsync(new LoginCommand(email, password), CancellationToken.None));

        Assert.Equal("invalid_credentials", error.Code);
        Assert.Equal("Invalid email or password.", error.Message);
    }

    private sealed class FakeUsers(params User[] users) : IUserRepository
    {
        public Task<User?> FindByEmailAsync(Email email, CancellationToken ct) =>
            Task.FromResult(users.FirstOrDefault(u => u.Email == email.Value));
    }

    private sealed class FakeHasher : IPasswordHasher
    {
        public string Hash(string password) => $"hash:{password}";

        public bool Verify(string passwordHash, string password) => passwordHash == Hash(password);
    }

    private sealed class FakeTokens : IAccessTokenIssuer
    {
        public AccessToken Issue(User user) => new($"token-for-{user.Email}", DateTime.UtcNow.AddHours(1));
    }
}
