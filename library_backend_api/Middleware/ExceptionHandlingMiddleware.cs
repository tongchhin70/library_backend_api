using System.Net;
using library_backend_api.Exceptions;

namespace library_backend_api.Middleware;

// This middleware converts thrown exceptions into consistent API error responses.
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception while processing request.");
            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        // Map known application exceptions to the right HTTP status codes.
        context.Response.StatusCode = exception switch
        {
            NotFoundException => (int)HttpStatusCode.NotFound, //If the exception is NotFoundException, return 404.
            BadRequestException => (int)HttpStatusCode.BadRequest, //If the exception is BadRequestException, return 400. 
            ConflictException => (int)HttpStatusCode.Conflict, //If the exception is ConflictException, return 409.
            _ => (int)HttpStatusCode.InternalServerError //If anything else, return 500 Internal Server Error.
        };

        var message = context.Response.StatusCode == (int)HttpStatusCode.InternalServerError
            ? "An unexpected error occurred."
            : exception.Message;

        await context.Response.WriteAsJsonAsync(new { error = message });
    }
}
