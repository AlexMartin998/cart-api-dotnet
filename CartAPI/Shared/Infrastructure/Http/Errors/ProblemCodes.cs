namespace CartAPI.Shared.Infrastructure.Http.Errors;

public static class ProblemCodes
{
    public static string ForStatus(int status) => status switch
    {
        400 => "bad_request",
        401 => "unauthorized",
        403 => "forbidden",
        404 => "not_found",
        405 => "method_not_allowed",
        409 => "conflict",
        415 => "unsupported_media_type",
        _ => status >= 500 ? "internal_error" : "error",
    };
}
