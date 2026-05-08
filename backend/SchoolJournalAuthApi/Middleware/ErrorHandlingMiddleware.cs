using System.Net;
using System.Text.Json;

namespace SchoolJournalAuthApi.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger) 
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode code = HttpStatusCode.InternalServerError;
            string errorMessage = string.Empty;
            string errorCode = string.Empty;
            switch (ex) 
            {
                default:
                    _logger.LogError(ex, "Unknown exception has occured");
                    errorMessage = "An exception has occured";
                    errorCode = "UNKNOWN_ERROR";
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            var response = new ApiError
            {
                StatusCode = (int)code,
                ErrorCode = errorCode,
                Message = errorMessage
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
