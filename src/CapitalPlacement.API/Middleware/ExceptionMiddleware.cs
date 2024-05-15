using CapitalPlacement.API.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CapitalPlacement.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ValidationException ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "ValidationFailure",
                    Title = "Validation error",
                    Detail = "One or more validation errors has occured"
                };

                if (ex.Errors is not null)
                {
                    problemDetails.Extensions["errors"] = ex.Errors;
                }

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                await httpContext.Response.WriteAsJsonAsync(problemDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new ErrorMessage { Message = exception.Message };

            await context.Response.WriteAsJsonAsync(response);
        }

        public class ErrorMessage
        {
            public string Message { get; set; } = string.Empty;
        }
    }
}
