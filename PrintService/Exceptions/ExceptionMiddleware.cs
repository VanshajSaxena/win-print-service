using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PrintService.Exceptions
{
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionMiddleware> _logger = logger;
        private readonly IHostEnvironment _env = env;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }

        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode = ExceptionToHttpStatusCodeMapper.GetStatusCode(exception);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new Dictionary<string, object>
            {
                ["statusCode"] = statusCode,
                ["message"] = exception.Message
            };

            if (_env.IsDevelopment() && exception.StackTrace != null)
            {
                response["stackTrace"] = exception.StackTrace;
            }

            string json = JsonSerializer.Serialize(response);

            _logger.LogError(exception, "Exception Message: {}", exception.Message);
            await context.Response.WriteAsync(json);
        }
    }

    static class ExceptionToHttpStatusCodeMapper
    {
        private static readonly Dictionary<Type, HttpStatusCode> ExceptionMapping = new()
        {
            { typeof(PrintQueueNotFoundException), HttpStatusCode.NotFound},
            { typeof(JobNotFoundException), HttpStatusCode.NotFound},
            { typeof(ConversionFailedException), HttpStatusCode.BadRequest },
            { typeof(InvalidOperationException), HttpStatusCode.NotFound },
            { typeof(ArgumentException), HttpStatusCode.BadRequest }
        };

        public static HttpStatusCode GetStatusCode(Exception ex)
        {
            return ExceptionMapping.GetValueOrDefault(ex.GetType(), HttpStatusCode.InternalServerError);
        }
    }
}
