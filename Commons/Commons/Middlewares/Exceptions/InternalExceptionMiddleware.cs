using Commons.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Commons.Middlewares.Exceptions
{
    public class InternalExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<InternalExceptionMiddleware> _logger;

        public InternalExceptionMiddleware(ILogger<InternalExceptionMiddleware> logger)
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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            InternalException DIE = new InternalException(
                exception.Message
            );

            return context.Response.WriteAsync(JsonConvert.SerializeObject(DIE));
        }
    }
}
