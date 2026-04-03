using SchoolJournalApi.Exceptions;
using System.Net;
using System.Text.Json;

namespace SchoolJournalApi.Middleware
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
                case EntityInUseException:
                    code = HttpStatusCode.Conflict;
                    errorMessage = ex.Message;
                    errorCode = "ENTITY_IN_USE_ERROR";
                    break;
                case EntityNotFoundException:
                    code = HttpStatusCode.NotFound;
                    errorMessage = ex.Message;
                    errorCode = "ENTITY_NOT_FOUND_ERROR";
                    break;
                case EntityAddingException:
                    code = HttpStatusCode.Conflict;
                    errorCode = "ENTITY_ADDING_ERROR";
                    errorMessage = ex.Message;
                    break;
                case EntityHasStatusDiscrepancyException:
                    code = HttpStatusCode.Conflict;
                    errorCode = "STATUS_DISCREPANCY_ERROR";
                    errorMessage = ex.Message;
                    break;
                case EntityUpdateException:
                    code = HttpStatusCode.Conflict;
                    errorCode = "ENTITY_UPDATE_ERROR";
                    errorMessage = ex.Message;
                    break;
                case InvalidOperationException:
                    code = HttpStatusCode.Conflict;
                    errorCode = "INVALID_OPERATION_ERROR";
                    errorMessage = ex.Message;
                    break;
                case EntityAlreadyExistsException:
                    code = HttpStatusCode.Conflict;
                    errorCode = "ENTITY_ALREADY_EXISTS_ERROR";
                    errorMessage = ex.Message;
                    break;
                case EntityHasBusinessLogicConflictException:
                    code = HttpStatusCode.Conflict;
                    errorCode = "ENTITY_LOGIC_CONFLICT_ERROR";
                    errorMessage = ex.Message;
                    break;
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
