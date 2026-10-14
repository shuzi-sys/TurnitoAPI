using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using TurnitoAPI.Exceptions;
namespace TurnitoAPI.Exceptions
{
    public class APIExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<APIExceptionHandler> _logger;
        public APIExceptionHandler(ILogger<APIExceptionHandler> logger) => _logger = logger;
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception ex, CancellationToken ct) {
            var (status, title) = ex switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Not found"),
                ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
                BusinessRuleException => (StatusCodes.Status400BadRequest, "Invalid request"),
                _ => (StatusCodes.Status500InternalServerError, "Internal Error")
            };

            if (status == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(ex, "Unhandled exception");
            }

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = status == 500 ? null : ex.Message
            }, ct);

            return true;
        }
    }
}
