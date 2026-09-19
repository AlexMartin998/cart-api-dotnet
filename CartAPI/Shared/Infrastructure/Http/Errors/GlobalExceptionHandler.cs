using CartAPI.Shared.Domain.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Shared.Infrastructure.Http.Errors;

public sealed partial class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetails)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, code, detail) = Map(exception);

        if (status == StatusCodes.Status500InternalServerError)
            LogUnexpected(exception, httpContext.Request.Method, httpContext.Request.Path);
        else
            LogRejected(code, httpContext.Request.Method, httpContext.Request.Path, detail);

        httpContext.Response.StatusCode = status;
        var problem = new ProblemDetails
        {
            Status = status,
            Title = ReasonPhrases.GetReasonPhrase(status),
            Detail = detail,
        };
        problem.Extensions["code"] = code;

        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problem,
        });
    }

    private static (int Status, string Code, string Detail) Map(Exception exception) => exception switch
    {
        ValidationException e => (400, e.Code, e.Message),
        UnauthorizedException e => (401, e.Code, e.Message),
        NotFoundException e => (404, e.Code, e.Message),
        ConflictException e => (409, e.Code, e.Message),
        BadHttpRequestException bad => (bad.StatusCode, ProblemCodes.ForStatus(bad.StatusCode), "The request could not be read."),

        // 2601/2627: unique index violation.
        DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } } =>
            (409, "conflict", "The value already exists."),

        // Never leak internal messages; the details go to the log.
        _ => (500, "internal_error", "An unexpected error occurred."),
    };

    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception on {Method} {Path}")]
    private partial void LogUnexpected(Exception exception, string method, PathString path);

    [LoggerMessage(Level = LogLevel.Warning, Message = "{Code} on {Method} {Path}: {Detail}")]
    private partial void LogRejected(string code, string method, PathString path, string detail);
}
