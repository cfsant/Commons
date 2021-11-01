using Commons.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Commons.Middlewares.Exceptions
{
    public class DeveloperInternalExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<DeveloperInternalExceptionMiddleware> _logger;

        public DeveloperInternalExceptionMiddleware(ILogger<DeveloperInternalExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex}");
                await HandleExceptionAsync(context, ex);
            }
        }

        public static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            DeveloperInternalException DIE = new DeveloperInternalException(
                exception.Message,
                exception.InnerException,
                exception.StackTrace
            );

            return context.Response.WriteAsync(JsonConvert.SerializeObject(DIE));
        }
    }
}
