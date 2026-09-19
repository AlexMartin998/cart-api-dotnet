using System.Net;
using System.Net.Http.Json;

namespace CartAPI.Tests.Integration;

public sealed class AuthFlowTests : IDisposable
{
    private readonly ApiFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task Login_ReturnsATokenThatOpensPrivateEndpoints()
    {
        var client = await _factory.LoginAsync(ApiClient.Customer, ApiClient.CustomerPassword);

        var me = await (await client.GetAsync("/api/auth/me")).JsonAsync();

        Assert.Equal(ApiClient.Customer, me.GetProperty("email").GetString());
        Assert.Equal("Customer", me.GetProperty("role").GetString());
    }

    [Theory]
    [InlineData(ApiClient.Customer, "wrong-password")]
    [InlineData("nobody@cartapi.local", ApiClient.CustomerPassword)]
    public async Task Login_UnknownEmailAndWrongPassword_GetTheSame401(string email, string password)
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("/api/auth/login", new { email, password });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("invalid_credentials", await response.CodeAsync());
    }

    [Fact]
    public async Task Login_WithAnInvalidBody_IsAValidationError()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync("/api/auth/login", new { email = "x", password = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.JsonAsync();
        Assert.Equal("validation_error", problem.GetProperty("code").GetString());
        Assert.True(problem.GetProperty("errors").TryGetProperty("email", out _));
    }

    [Fact]
    public async Task PrivateEndpoints_AreClosedByDefault()
    {
        var response = await _factory.CreateClient().GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("unauthorized", await response.CodeAsync());
    }

    [Fact]
    public async Task AnUnknownRoute_Is404_Not401()
    {
        var response = await _factory.CreateClient().GetAsync("/api/does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task TheApiDocumentation_IsPublic()
    {
        var client = _factory.CreateClient();

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/openapi/v1.json")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/swagger/index.html")).StatusCode);
    }
}
