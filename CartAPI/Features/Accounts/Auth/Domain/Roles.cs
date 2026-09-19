namespace CartAPI.Features.Accounts.Auth.Domain;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Customer = "Customer";

    public const int MaxLength = 20;

    public static bool IsKnown(string role) => role is Admin or Customer;
}
