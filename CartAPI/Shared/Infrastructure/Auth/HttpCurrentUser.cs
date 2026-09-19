using System.Globalization;
using Microsoft.IdentityModel.JsonWebTokens;

namespace CartAPI.Shared.Infrastructure.Auth;

internal sealed class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public int Id =>
        int.TryParse(accessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, NumberStyles.None,
            CultureInfo.InvariantCulture, out var id)
            ? id
            : throw new InvalidOperationException("The request has no authenticated user.");
}
