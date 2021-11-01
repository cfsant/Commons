using Commons.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Commons.Extensions
{
    public static class RequestManagementExtencions
    {
		public static IServiceCollection AddRequestManagementExceptionMiddleware(this IServiceCollection services)
		{
			//services.AddTransient<InternalExceptionMiddleware>();
			//services.AddTransient<DeveloperInternalExceptionMiddleware>();
			//
			return services.AddTransient<RequestManagementMeddleware>();
		}

		public static void UseRequestManagementMiddleware(this IApplicationBuilder app, string env)
		{
			app.UseMiddleware<RequestManagementMeddleware>();

			//switch (env.ToLower())
			//{
			//	case "development":
			//		app.UseMiddleware<RequestManagementMeddleware>();
			//		break;
			//
			//	case "production":
			//		app.UseMiddleware<RequestManagementMeddleware>();
			//		break;
			//
			//	default:
			//		app.UseMiddleware<RequestManagementMeddleware>();
			//		break;
			//}
		}
	}
}
