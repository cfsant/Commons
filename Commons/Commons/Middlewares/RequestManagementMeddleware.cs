using Commons.Middlewares.Exceptions;
using Commons.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Commons.Middlewares
{
    public class RequestManagementMeddleware : IMiddleware
    {
        private readonly ILogger<RequestManagementMeddleware> _logger;
        private List<KeyValuePair<string, StringValues>> _headers;
        private string _authorization;
        private string _path;
        private JwtSecurityToken _token;

        public RequestManagementMeddleware(ILogger<RequestManagementMeddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                HeadersManagement(context);
                PathManagement();
                AuthorizationManagement();

                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex}");

                await DeveloperInternalExceptionMiddleware.HandleExceptionAsync(context, ex);
            }
        }

        private void HeadersManagement(HttpContext context)
        {
            _headers = context.Request.Headers.ToList();
            if (_headers == null || _headers.Count < 1)
            {
                throw new Exception("RequestManagementMeddleware(InvokeAsync) throw this exception: Request no headers to read.");
            }
        }

        private void PathManagement()
        {
            _path = _headers.Find(x => x.Key == ":path").Value.ToString();
        }

        private void AuthorizationManagement()
        {
            // This route can be acess with guest
            if (_path == "/Profile/Authenticate" || _path == "/Profile/Create" || _path == "/Session/FetchByToken" || _path == "/Profile/GetDefaultOwnerId")
            {
                return;
            }

            _authorization = _headers.Find(x => x.Key == "Authorization").Value.ToString().Replace("Bearer ", string.Empty);
            if (_authorization == null || _authorization == default)
            {
                throw new Exception("RequestManagementMeddleware(AuthorizationManagement) throw this exception: No request header 'Authorization' to read.");
            }

            _token = ProfileAutheticationTokenServices.ReadToken(_authorization);
            if (_token == null || _token == default)
            {
                throw new Exception("Invalid token.");
            }

            if (DateTime.UtcNow > _token.ValidTo)
            {
                throw new Exception("Token expires in UTC: " + _token.ValidTo + " — Local: " + _token.ValidTo.ToLocalTime() + ".");
            }
        }
    }
}
