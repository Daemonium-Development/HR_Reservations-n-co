using DebugDiner.Domain.Abstractions;
using DebugDiner.Domain.Configurations;
using DebugDiner.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DebugDiner.Infrastructure;

public static class ServiceExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure()
        {
            services.AddOptions<DatabaseOptions>()
                .BindConfiguration(DatabaseOptions.SectionName);
            
            services.AddSingleton<IDataService, DataService>();
            
            return services;
        }
    }
}