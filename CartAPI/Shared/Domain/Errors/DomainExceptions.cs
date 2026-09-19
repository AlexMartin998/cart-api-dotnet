namespace CartAPI.Shared.Domain.Errors;

public sealed class ValidationException(string code, string message) : DomainException(code, message);

public sealed class NotFoundException(string code, string message) : DomainException(code, message);

public sealed class ConflictException(string code, string message) : DomainException(code, message);

public sealed class UnauthorizedException(string code, string message) : DomainException(code, message);
