using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CartAPI.Shared.Infrastructure.Auth;

// No fallback policy without an endpoint: unknown routes stay 404.
internal sealed class EndpointFallbackPolicyProvider(IOptions<AuthorizationOptions> options, IHttpContextAccessor accessor)
    : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _inner = new(options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _inner.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName) => _inner.GetPolicyAsync(policyName);

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        accessor.HttpContext?.GetEndpoint() is null
            ? Task.FromResult<AuthorizationPolicy?>(null)
            : _inner.GetFallbackPolicyAsync();
}
