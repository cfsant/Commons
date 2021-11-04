using Commons.Middlewares.Exceptions;
using Commons.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
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
        private readonly IConfiguration _configuration;
        private List<KeyValuePair<string, StringValues>> _headers;
        private string _authorization;
        private string _path;
        private JwtSecurityToken _token;
        private IEnumerable<IConfigurationSection> _current { get; set; }

        public RequestManagementMeddleware(ILogger<RequestManagementMeddleware> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
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
            if (!this.AuthorizationManagementValidate())
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

        private bool AuthorizationManagementValidate()
        {
            bool result = true;

            this._current = _configuration.GetChildren();
            if (this._current == null || this._current == default)
            {
                return result;
            }

            foreach (var config in this._current)
            {
                if (config.Key == null)
                {
                    continue;
                }
            }

            var sGuestEndpoints = _configuration["GuestEndpoints"];

            var serializedGuestEndpoints = _configuration?.GetSection("GuestEndpoints")?.Value;
            if (sGuestEndpoints == null || sGuestEndpoints == default)
            {
                return result;
            }

            var guestEndpoints = JsonConvert.DeserializeObject<string[]>(sGuestEndpoints);
            if (guestEndpoints == null || guestEndpoints == default)
            {
                return result;
            }

            guestEndpoints.Any(endpoint => {
                if (_path == endpoint)
                {
                    result = false;

                    return true;
                }

                return false;
            });

            return result;
        }
    }
}
