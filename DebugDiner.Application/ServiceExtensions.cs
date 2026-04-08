using Microsoft.Extensions.DependencyInjection;

namespace DebugDiner.Application;

public static class ServiceExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services.AddSingleton<IAuthService, AuthService>();
            return services;
        }
    }
}