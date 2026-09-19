namespace FitBodyApp.Application.Common;

public static class ErrorCodes
{
    public const string ValidationError = "VALIDATION_ERROR";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string NotFound = "NOT_FOUND";
    public const string Conflict = "CONFLICT";
    public const string UnprocessableEntity = "UNPROCESSABLE_ENTITY";
    public const string RateLimited = "RATE_LIMITED";
    public const string InternalError = "INTERNAL_ERROR";
}

public class AppException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }

    public AppException(string code, string message, int statusCode) : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }

    public static AppException ValidationError(string message) => new(ErrorCodes.ValidationError, message, 400);
    public static AppException Unauthorized(string message = "Unauthorized") => new(ErrorCodes.Unauthorized, message, 401);
    public static AppException Forbidden(string message = "Forbidden") => new(ErrorCodes.Forbidden, message, 403);
    public static AppException NotFound(string message = "Not found") => new(ErrorCodes.NotFound, message, 404);
    public static AppException Conflict(string message) => new(ErrorCodes.Conflict, message, 409);
    public static AppException Unprocessable(string message) => new(ErrorCodes.UnprocessableEntity, message, 422);
}
