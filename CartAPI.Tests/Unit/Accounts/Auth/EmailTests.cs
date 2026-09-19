using CartAPI.Features.Accounts.Auth.Domain.ValueObjects;
using CartAPI.Shared.Domain.Errors;

namespace CartAPI.Tests.Unit.Accounts.Auth;

public sealed class EmailTests
{
    [Fact]
    public void Create_NormalizesToLowerCaseWithoutSpaces() =>
        Assert.Equal("ana@x.com", Email.Create("  Ana@X.com ").Value);

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ana")]
    [InlineData("@x.com")]
    [InlineData("ana@")]
    public void Create_RejectsInvalidEmails(string? email) =>
        Assert.Equal("invalid_email", Assert.Throws<ValidationException>(() => Email.Create(email)).Code);
}
