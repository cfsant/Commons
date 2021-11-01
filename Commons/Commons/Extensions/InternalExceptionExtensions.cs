using Commons.Middlewares.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Commons.Extensions
{
    public static class InternalExceptionExtensions
    {
		public static IServiceCollection AddInternalExceptionMiddleware(this IServiceCollection services)
		{
			services.AddTransient<InternalExceptionMiddleware>();
			services.AddTransient<DeveloperInternalExceptionMiddleware>();

			return services.AddTransient<ProductionInternalExceptionMiddleware>();
		}

		public static void UseInternalExceptionMiddleware(this IApplicationBuilder app, string env)
		{
            switch (env.ToLower())
            {
				case "development":
					app.UseMiddleware<DeveloperInternalExceptionMiddleware>();
					break;

				case "production":
					app.UseMiddleware<ProductionInternalExceptionMiddleware>();
					break;

				default:
					app.UseMiddleware<InternalExceptionMiddleware>();
					break;
			}
		}
	}
}
