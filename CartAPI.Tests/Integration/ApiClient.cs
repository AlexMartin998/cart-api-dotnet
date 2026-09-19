using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace CartAPI.Tests.Integration;

internal static class ApiClient
{
    public const string Customer = "cliente@cartapi.local";
    public const string CustomerPassword = "Cliente123!";
    public const string Admin = "admin@cartapi.local";
    public const string AdminPassword = "Admin123!";

    public static async Task<HttpClient> LoginAsync(this ApiFactory factory, string email, string password)
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();

        var token = (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("accessToken").GetString();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static async Task<JsonElement> JsonAsync(this HttpResponseMessage response) =>
        await response.Content.ReadFromJsonAsync<JsonElement>();

    public static async Task<string> CodeAsync(this HttpResponseMessage response) =>
        (await response.JsonAsync()).GetProperty("code").GetString()!;
}
