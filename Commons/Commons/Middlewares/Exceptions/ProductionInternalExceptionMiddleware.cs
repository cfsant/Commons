using Commons.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Commons.Middlewares.Exceptions
{
    public class ProductionInternalExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ProductionInternalExceptionMiddleware> _logger;

        public ProductionInternalExceptionMiddleware(ILogger<ProductionInternalExceptionMiddleware> logger)
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
            ProductionInternalException PIE = null;
            PropertyInfo prop = null;
            string message = null;
            string detail = null;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (exception.InnerException == null || exception.InnerException == default)
            {
                message = "An internal error has occurred that cannot be handled.";
                detail = "No error details.";

                PIE = new ProductionInternalException(message, detail);
                return context.Response.WriteAsync(JsonConvert.SerializeObject(PIE));
            }

            prop = exception.InnerException.GetType().GetProperty("Detail");
            if (prop == null || prop == default)
            {
                detail = "No error details.";
            }

            ProductionInternalException DIE = new ProductionInternalException(
                exception.InnerException.Message,
                detail
            );

            return context.Response.WriteAsync(JsonConvert.SerializeObject(DIE));
        }
    }
}
