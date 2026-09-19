using CartAPI.Features.Accounts.Auth.Domain;

namespace CartAPI.Features.Accounts.Auth.Application;

public sealed record UserDto(int Id, string Email, string FullName, string Role)
{
    public static UserDto From(User user) => new(user.Id, user.Email, user.FullName, user.Role);
}
