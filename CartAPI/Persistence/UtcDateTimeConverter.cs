using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CartAPI.Persistence;

// SQL Server drops DateTime.Kind: read everything back as UTC.
internal sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
    v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
