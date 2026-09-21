using Hope_National_Hospital.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace Hope_National_Hospital.Middleware
{
    public class GlobalExceptionMiddlewere
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddlewere> _logger;

        public GlobalExceptionMiddlewere(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddlewere> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception. Method: {Method}, Path: {Path}, TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            string message;

            if (exception is AppException appException)
            {
                statusCode = appException.StatusCode;
                message = appException.Message;
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "An unexpected error occurred.";
            }

            context.Response.StatusCode = statusCode;

            var response = new
            {
                success = false,
                statusCode,
                message,
                timestamp = DateTime.UtcNow,
                traceId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}