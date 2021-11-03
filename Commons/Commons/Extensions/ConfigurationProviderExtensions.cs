using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Extensions
{
    public static class ConfigurationProviderExtensions
    {
		public static IServiceCollection AddConfigurationProviderMiddleware(this IServiceCollection services, Type configurationProviderModel)
		{
			return services.AddTransient(configurationProviderModel);
		}
	}
}
