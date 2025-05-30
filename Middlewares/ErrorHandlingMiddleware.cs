using System.Net;
using System.Text.Json;

namespace BlogApp.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            int statusCode;
            string errorMessage;

            switch (ex)
            {
                case ApplicationException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    errorMessage = ex.Message;
                    break;

                case UnauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    errorMessage = "Unauthorized";
                    break;

                case KeyNotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    errorMessage = "Resource not found";
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    errorMessage = "An unexpected error occurred.";
                    break;
            }

            _logger.LogError(ex, "An error occurred while processing the request: {Message}", ex.Message);
            response.StatusCode = statusCode;

            var errorResponse = new
            {
                statusCode,
                message = errorMessage,
                // Only show full exception details in development
                details = _env.IsDevelopment() ? ex.ToString() : null
            };

            var json = JsonSerializer.Serialize(errorResponse,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await response.WriteAsync(json);
        }
    }
}
