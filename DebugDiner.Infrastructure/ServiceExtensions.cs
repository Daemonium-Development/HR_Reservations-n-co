using DebugDiner.Domain.Abstractions;
using DebugDiner.Domain.Configurations;
using DebugDiner.Infrastructure.Repositories;
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

            services.AddSingleton<IArrangementRepository, ArrangementRepository>();
            services.AddSingleton<IMenuRepository, MenuRepository>();
            services.AddSingleton<IReservationRepository, ReservationRepository>();
            services.AddSingleton<ITableRepository, TableRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();

            return services;
        }
    }
}